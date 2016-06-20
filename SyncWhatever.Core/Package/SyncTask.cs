using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;

namespace SyncWhatever.Core.Package
{
    public class SyncTask<TEntityA, TEntityB> where TEntityA : class where TEntityB : class
    {
        protected readonly string SyntTaskId;
        protected readonly IDataSource<TEntityA> DataSource;
        protected readonly IDataTarget<TEntityA, TEntityB> DataTarget;
        protected readonly Action<string, string, TEntityA, TEntityB> ExecuteNestedTasks;
        protected readonly string ParentContextKey;
        protected readonly ISyncKeyMapStorage SyncKeyMapStorage;
        protected readonly ISyncStateStorage SyncStateStorage;

        public SyncTask(
            string syntTaskId,
            IDataSource<TEntityA> dataSource,
            IDataTarget<TEntityA, TEntityB> dataTarget,
            ISyncStateStorage syncStateStorage,
            ISyncKeyMapStorage syncKeyMapStorage,
            Action<string, string, TEntityA, TEntityB> executeNestedTasks = null,
            string parentContextKey = null
            )
        {
            SyntTaskId = syntTaskId;
            DataSource = dataSource;
            DataTarget = dataTarget;
            SyncStateStorage = syncStateStorage;
            SyncKeyMapStorage = syncKeyMapStorage;
            ExecuteNestedTasks = executeNestedTasks;
            ParentContextKey = parentContextKey;
        }


        public ILog Log => LogManager.GetLogger(Context);

        private string Context => $"[{SyntTaskId}]:[{ParentContextKey}]";

        public void Execute()
        {
            Log.Debug($"==== Sync started ====");

            var stateChanges = GetStateChanges();

            foreach (var stateChange in stateChanges)
            {
                ResolveSourceKey(stateChange);
                LookupSourceItem(stateChange);

                ResolveTargetKey(stateChange);
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
            var lastStates = SyncStateStorage.GetStates(Context).ToList();

            foreach (var currentState in currentStates)
            {
                var lastState = lastStates.FirstOrDefault(x => x.Key == currentState.Key);

                if (lastState == null)
                {
                    var insert = new StateChange<TEntityA, TEntityB>
                    {
                        CurrentSyncState = currentState
                    };
                    result.Add(insert);
                    Log.Debug(
                        $"Theoretical INSERT: CurrentState = '{insert.CurrentSyncState}', LastState = '{insert.LastSyncState}'");
                }
                else
                {
                    if (lastState.Hash != currentState.Hash)
                    {
                        var update = new StateChange<TEntityA, TEntityB>
                        {
                            CurrentSyncState = currentState,
                            LastSyncState = lastState
                        };
                        result.Add(update);
                        Log.Debug(
                            $"Theoretical UPDATE: CurrentState = '{update.CurrentSyncState}', LastState = '{update.LastSyncState}'");
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
                        LastSyncState = lastState
                    };
                    result.Add(delete);
                    Log.Debug(
                        $"Theoretical DELETE: CurrentState = '{delete.CurrentSyncState}', LastState = '{delete.LastSyncState}'");
                }
            }
            return result;
        }

        private void ResolveSourceKey(StateChange<TEntityA, TEntityB> stateChange)
        {
            stateChange.SourceKey = stateChange.CurrentSyncState?.Key ?? stateChange.LastSyncState?.Key;
        }

        private void ResolveTargetKey(StateChange<TEntityA, TEntityB> stateChange)
        {
            if (stateChange.SourceKey == null)
                return;

            stateChange.SyncKeyMap = SyncKeyMapStorage.GetBySourceKey(Context, stateChange.SourceKey);
            stateChange.TargetKey = stateChange.SyncKeyMap?.TargetKey;

            if (stateChange.SourceItem != null && stateChange.TargetItem == null)
            {
                stateChange.TargetKey = DataTarget.GetKeyBySourceItem(stateChange.SourceItem);
            }
        }

        private void LookupSourceItem(StateChange<TEntityA, TEntityB> stateChange)
        {
            if (stateChange.SourceKey == null)
                return;

            stateChange.SourceItem = DataSource.GetByKey(stateChange.SourceKey);
        }

        private void LookupTargetItem(StateChange<TEntityA, TEntityB> stateChange)
        {
            if (stateChange.TargetKey == null)
                return;

            stateChange.TargetItem = DataTarget.GetByKey(stateChange.TargetKey);
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
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey, stateChange.SourceItem, stateChange.TargetItem);
                    break;
                case OperationEnum.Update:
                    stateChange.TargetKey = DataTarget.Update(stateChange.SourceItem, stateChange.TargetItem);
                    Log.Debug($"UPDATE: SourceKey = '{stateChange.SourceKey}', TargetKey = '{stateChange.TargetKey}'");
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey, stateChange.SourceItem, stateChange.TargetItem);
                    break;
                case OperationEnum.Delete:
                    Log.Debug($"DELETE: SourceKey = '{stateChange.SourceKey}', TargetKey = '{stateChange.TargetKey}'");
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey, stateChange.SourceItem, stateChange.TargetItem);
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
                    if (stateChange.SyncKeyMap == null)
                    {
                        SyncKeyMapStorage.Create(Context, stateChange.SourceKey, stateChange.TargetKey);
                    }
                    else
                    {
                        var syncKeyMap = stateChange.SyncKeyMap;
                        syncKeyMap.TargetKey = stateChange.TargetKey;
                        SyncKeyMapStorage.Update(syncKeyMap);
                    }
                    break;

                case OperationEnum.None:
                case OperationEnum.Delete:
                    if (stateChange.SyncKeyMap != null)
                    {
                        SyncKeyMapStorage.Delete(stateChange.SyncKeyMap);
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

                    if (stateChange.LastSyncState == null)
                    {
                        SyncStateStorage.Create(Context, stateChange.CurrentSyncState.Key,
                            stateChange.CurrentSyncState.Hash);
                    }
                    else
                    {
                        var syncState = stateChange.LastSyncState;
                        syncState.Context = Context;
                        syncState.Hash = stateChange.CurrentSyncState.Hash;
                        SyncStateStorage.Update(syncState);
                    }
                    break;

                case OperationEnum.None:
                case OperationEnum.Delete:
                    if (stateChange.LastSyncState != null)
                    {
                        SyncStateStorage.Delete(stateChange.LastSyncState);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}