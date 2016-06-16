using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;

namespace SyncItEasy.Core.Package
{
    public class SyncTask<TEntityA, TEntityB> where TEntityA : class where TEntityB : class
    {
        protected readonly IDataSource<TEntityA> DataSource;
        protected readonly IDataTarget<TEntityA, TEntityB> DataTarget;
        protected readonly Action<string, string> ExecuteNestedTasks;
        protected readonly IKeyMapStorage KeyMapStorage;
        protected readonly string PartitionKey;
        protected readonly IStateStorage StateStorage;

        public SyncTask(
            IDataSource<TEntityA> dataSource,
            IDataTarget<TEntityA, TEntityB> dataTarget,
            IStateStorage stateStorage,
            IKeyMapStorage keyMapStorage,
            Action<string, string> executeNestedTasks,
            string partitionKey
            )
        {
            DataSource = dataSource;
            DataTarget = dataTarget;
            StateStorage = stateStorage;
            KeyMapStorage = keyMapStorage;
            ExecuteNestedTasks = executeNestedTasks;
            PartitionKey = partitionKey;
        }


        public ILog Log => LogManager.GetLogger(ProcessKey);

        private string ProcessKey => $"{typeof(TEntityA).Name}=>{typeof(TEntityB).Name}:{PartitionKey ?? "root"}";

        public void Execute()
        {
            Log.Debug($"==== Sync started ====");

            var stateChanges = GetStateChanges();

            foreach (var stateChange in stateChanges)
            {
                ResolveSourceKey(stateChange);
                ResolveTargetKey(stateChange);

                LookupSourceItem(stateChange);
                LookupTargetItem(stateChange);

                DetectDataOperation(stateChange);
                PerformDataOperation(stateChange);

                UpdateSyncMap(stateChange);
                UpdateSyncState(stateChange);
            }

            Log.Debug($"==== Sync done ====");
        }

        protected virtual List<StateChange<TEntityA, TEntityB>> GetStateChanges()
        {
            var result = new List<StateChange<TEntityA, TEntityB>>();

            var currentStates = DataSource.GetStates().ToList();
            var lastStates = StateStorage.GetStates(ProcessKey).ToList();

            foreach (var currentState in currentStates)
            {
                var lastState = lastStates.FirstOrDefault(x => x.Key == currentState.Key);

                if (lastState == null)
                {
                    var insert = new StateChange<TEntityA, TEntityB>
                    {
                        CurrentState = currentState
                    };
                    result.Add(insert);
                    Log.Debug(
                        $"Theoretical INSERT: CurrentState = '{insert.CurrentState}', LastState = '{insert.LastState}'");
                }
                else
                {
                    if (lastState.Hash != currentState.Hash)
                    {
                        var update = new StateChange<TEntityA, TEntityB>
                        {
                            CurrentState = currentState,
                            LastState = lastState
                        };
                        result.Add(update);
                        Log.Debug(
                            $"Theoretical UPDATE: CurrentState = '{update.CurrentState}', LastState = '{update.LastState}'");
                    }
                }
            }

            foreach (var lastState in lastStates)
            {
                var currentState = currentStates.SingleOrDefault(x => x.Key == lastState.Key);

                if (currentState == null)
                {
                    var delete = new StateChange<TEntityA, TEntityB>
                    {
                        LastState = lastState
                    };
                    result.Add(delete);
                    Log.Debug(
                        $"Theoretical DELETE: CurrentState = '{delete.CurrentState}', LastState = '{delete.LastState}'");
                }
            }
            return result;
        }

        private void ResolveSourceKey(StateChange<TEntityA, TEntityB> stateChange)
        {
            stateChange.SourceKey = stateChange.CurrentState?.Key ?? stateChange.LastState?.Key;
        }

        private void ResolveTargetKey(StateChange<TEntityA, TEntityB> stateChange)
        {
            if (stateChange.SourceKey == null)
                return;

            stateChange.SyncMap = KeyMapStorage.GetBySourceKey(ProcessKey, stateChange.SourceKey);
            ;
            stateChange.TargetKey = stateChange.SyncMap?.TargetKey;
        }

        private void LookupSourceItem(StateChange<TEntityA, TEntityB> stateChange)
        {
            if (stateChange.SourceKey == null)
                return;

            stateChange.SourceItem = DataSource.GetByKey(stateChange.SourceKey);
        }

        private void LookupTargetItem(StateChange<TEntityA, TEntityB> stateChange)
        {
            if (stateChange.TargetKey != null)
            {
                stateChange.TargetItem = DataTarget.GetByKey(stateChange.TargetKey);
                return;
            }

            if (stateChange.SourceItem != null)
            {
                stateChange.TargetItem = DataTarget.GetBySourceItem(stateChange.SourceItem);
            }
        }

        private void DetectDataOperation(StateChange<TEntityA, TEntityB> stateChange)
        {
            if (stateChange.SourceItem != null && stateChange.TargetItem == null)
            {
                stateChange.Operation = OperationEnum.Insert;
                return;
            }
            if (stateChange.SourceItem != null && stateChange.TargetItem != null)
            {
                stateChange.Operation = OperationEnum.Update;
                return;
            }

            if (stateChange.SourceItem == null && stateChange.TargetItem != null)
            {
                Log.Debug($"DELETE: TargetKey = '{stateChange.TargetKey}'");
                stateChange.Operation = OperationEnum.Delete;
            }
        }

        private void PerformDataOperation(StateChange<TEntityA, TEntityB> stateChange)
        {
            switch (stateChange.Operation)
            {
                case OperationEnum.Insert:
                    stateChange.TargetKey = DataTarget.Insert(stateChange.SourceItem);
                    Log.Debug($"INSERT: SourceKey = '{stateChange.SourceKey}', TargetKey = '{stateChange.TargetKey}'");
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey);
                    break;
                case OperationEnum.Update:
                    stateChange.TargetKey = DataTarget.Update(stateChange.SourceItem, stateChange.TargetItem);
                    Log.Debug($"UPDATE: SourceKey = '{stateChange.SourceKey}', TargetKey = '{stateChange.TargetKey}'");
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey);
                    break;
                case OperationEnum.Delete:
                    Log.Debug($"DELETE: SourceKey = '{stateChange.SourceKey}', TargetKey = '{stateChange.TargetKey}'");
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey);
                    DataTarget.Delete(stateChange.TargetItem);
                    stateChange.TargetKey = null;
                    break;
                case OperationEnum.None:
                    break;
            }
        }

        private void UpdateSyncMap(StateChange<TEntityA, TEntityB> stateChange)
        {
            switch (stateChange.Operation)
            {
                case OperationEnum.Insert:
                case OperationEnum.Update:
                    var syncMap = stateChange.SyncMap ?? SyncMap.Create(ProcessKey, stateChange.SourceKey);
                    syncMap.TargetKey = stateChange.TargetKey;
                    KeyMapStorage.CreateOrUpdate(syncMap);
                    break;

                case OperationEnum.None:
                case OperationEnum.Delete:
                    if (stateChange.SyncMap != null)
                    {
                        KeyMapStorage.Delete(stateChange.SyncMap);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateSyncState(StateChange<TEntityA, TEntityB> stateChange)
        {
            switch (stateChange.Operation)
            {
                case OperationEnum.Insert:
                case OperationEnum.Update:

                    var syncState = stateChange.LastState ?? stateChange.CurrentState;
                    syncState.ProcessKey = ProcessKey;
                    syncState.Hash = stateChange.CurrentState.Hash;
                    StateStorage.CreateOrUpdate(syncState);
                    break;

                case OperationEnum.None:
                case OperationEnum.Delete:
                    if (stateChange.LastState != null)
                    {
                        StateStorage.Delete(stateChange.LastState);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}