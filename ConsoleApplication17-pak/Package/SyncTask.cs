using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication17_pak.Package
{
    public class SyncTask<TEntityA, TEntityB> where TEntityA : class where TEntityB : class
    {
        private readonly ICurrentStateProvider _aCurrentStateProvider;

        private readonly ILastStateProvider _aLastStateProvider;

        private readonly IEntityProvider<TEntityA> _aEntityProvider;
        private readonly IEntityProvider<TEntityB> _bEntityProvider;

        private readonly IFallbackEntityProvider<TEntityB, TEntityA> _bFallbackProvider;

        private readonly IEntityPersister<TEntityB, TEntityA> _bEntityPersister;

        private readonly ISyncMapProvider _syncMapProvider;
        private readonly Action<string, string> _nestedTasks;
        private readonly string _partitionKey;

        public SyncTask(
            // gets current state data (Storage?)
            ICurrentStateProvider aCurrentStateProvider,
            // DDS?
            ILastStateProvider aLastStateProvider,

            IEntityProvider<TEntityA> aEntityProvider,
            IEntityProvider<TEntityB> bEntityProvider,

            IFallbackEntityProvider<TEntityB, TEntityA> bFallbackProvider,
            IEntityPersister<TEntityB, TEntityA> bEntityPersister,

            ISyncMapProvider syncMapProvider,

            Action<string, string> nestedTasks
            )
        {
            _aCurrentStateProvider = aCurrentStateProvider;
            _aLastStateProvider = aLastStateProvider;

            _aEntityProvider = aEntityProvider;
            _bEntityProvider = bEntityProvider;

            _bFallbackProvider = bFallbackProvider;

            _bEntityPersister = bEntityPersister;

            _syncMapProvider = syncMapProvider;
            _nestedTasks = nestedTasks;
        }

        public void Execute()
        {
            var currentStates = _aCurrentStateProvider.GetStates();
            var lastStates = _aLastStateProvider.GetStates();
            var stateChanges = GetStateChanges(currentStates, lastStates).ToList();

            var syncMaps = _syncMapProvider.GetSyncMap();

            foreach (var hashChange in stateChanges)
            {
                var aKey = hashChange.CurrentState?.Key ?? hashChange.LastState?.Key;

                var aSyncMap = syncMaps
                    .SingleOrDefault(x => x.AKey == aKey);

                var bKey = aSyncMap?.BKey;

                TEntityA aItem = null;
                TEntityB bItem = null;

                if (aKey != null)
                {
                    aItem = _aEntityProvider.GetByKey(aKey);
                }

                if (bKey != null)
                {
                    bItem = _bEntityProvider.GetByKey(bKey);
                }

                if (bItem == null && aItem != null)
                {
                    bItem = _bFallbackProvider?.GetBySourceEntity(aItem);
                }

                // process changes
                if (aItem != null && bItem == null)
                {
                    bKey = _bEntityPersister.Insert(aItem);
                    _nestedTasks?.Invoke(aKey, bKey);
                }
                if (aItem != null && bItem != null)
                {
                    bKey = _bEntityPersister.Update(aItem, bItem);
                    _nestedTasks?.Invoke(aKey, bKey);
                }

                if (aItem == null && bItem != null)
                {
                    _nestedTasks?.Invoke(aKey, bKey);
                    _bEntityPersister.Delete(bItem);
                    bKey = null;
                }

                if (aItem == null && bItem == null)
                {
                    bKey = null;
                }

                // create or update sync map
                aSyncMap = aSyncMap ?? new SyncMap();
                aSyncMap.AKey = aKey;
                aSyncMap.BKey = bKey;
                _syncMapProvider.StoreSyncMap(aSyncMap);

                // create or update state
                var hash = hashChange.CurrentState ?? new State<TEntityA>();
                hash.Key = aKey;
                hash.Hash = hashChange.CurrentState?.Hash;
                _aLastStateProvider.SaveState(hash);
            }
        }

        private IEnumerable<IStateChange> GetStateChanges(IEnumerable<IState> aStates, IEnumerable<IState> lastStates)
        {
            foreach (var aState in aStates)
            {
                var lastState = lastStates
                    .FirstOrDefault(x => x.Key == aState.Key);

                if (lastState?.Hash != aState.Hash)
                {
                    yield return new StateChange
                    {
                        LastState = lastState,
                        CurrentState = aState
                    };
                }
            }

            foreach (var lastHash in lastStates)
            {
                var aHash = aStates.FirstOrDefault(x => x.Key == lastHash.Key);

                if (aHash == null)
                {
                    yield return new StateChange
                    {
                        LastState = lastHash
                    };
                }
            }
        }
    }
}