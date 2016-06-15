using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication17_pak.Package
{
    public class SyncTask<TEntityA, TEntityB> where TEntityA : class where TEntityB : class
    {
        protected readonly IDataSource<TEntityA> DataSource;
        protected readonly IDataTarget<TEntityA, TEntityB> DataTarget;
        protected readonly IKeyMapStorage KeyMapStorage;
        protected readonly Action<string, string> ExecuteNestedTasks;
        protected readonly IStateStorage StateStorage;

        public SyncTask(
            IDataSource<TEntityA> dataSource,
            IDataTarget<TEntityA, TEntityB> dataTarget,
            IStateStorage stateStorage,
            IKeyMapStorage keyMapStorage,
             Action<string, string> executeNestedTasks
            )
        {
            DataSource = dataSource;
            DataTarget = dataTarget;
            StateStorage = stateStorage;
            KeyMapStorage = keyMapStorage;
            ExecuteNestedTasks = executeNestedTasks;
        }

        public void Execute()
        {
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
        }

        protected virtual List<StateChange<TEntityA, TEntityB>> GetStateChanges()
        {
            var result = new List<StateChange<TEntityA, TEntityB>>();

            var currentStates = DataSource.GetStates().ToList();
            var lastStates = StateStorage.GetStates().ToList();

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
                    }
                }
            }

            foreach (var lastState in lastStates)
            {
                var currentState = currentStates.FirstOrDefault(x => x.Key == lastState.Key);

                if (currentState == null)
                {
                    var delete = new StateChange<TEntityA, TEntityB>
                    {
                        LastState = lastState
                    };
                    result.Add(delete);
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

            stateChange.SyncMap = KeyMapStorage.GetBySourceKey(stateChange.SourceKey);
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
                stateChange.Operation = OperationEnum.Delete;
            }
        }

        private void PerformDataOperation(StateChange<TEntityA, TEntityB> stateChange)
        {
            switch (stateChange.Operation)
            {
                case OperationEnum.Insert:
                    stateChange.TargetKey = DataTarget.Insert(stateChange.SourceItem);
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey);
                    break;
                case OperationEnum.Update:
                    stateChange.TargetKey = DataTarget.Update(stateChange.SourceItem, stateChange.TargetItem);
                    ExecuteNestedTasks?.Invoke(stateChange.SourceKey, stateChange.TargetKey);
                    break;
                case OperationEnum.Delete:
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
                    var syncMap = stateChange.SyncMap ??
                                  new SyncMap { Id = Guid.NewGuid(), Key = stateChange.SourceKey };
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