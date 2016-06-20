using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using SyncWhatever.Core.Package;

namespace SyncWhatever.Components.EPiServer.SyncState
{
    public class DdsSyncStateStorage : ISyncStateStorage
    {
        private readonly DynamicDataStore _store;

        public DdsSyncStateStorage()
        {
            _store = typeof(DdsSyncState).GetStore() ?? typeof(DdsSyncState).CreateStore();
        }

        public IEnumerable<ISyncState> GetStates(string context)
        {
            return _store
                .Items<DdsSyncState>()
                .Where(x => x.Context == context);
        }

        public void Create(string context, string key, string hash)
        {
            var ddsSyncState = new DdsSyncState
            {
                Id = Identity.NewIdentity(),
                Context = context,
                KeyValue = key,
                Hash = hash
            };
            _store.Save(ddsSyncState);
        }

        public void Update(ISyncState syncState)
        {
            var ddsSyncState = syncState as DdsSyncState;
            _store.Save(ddsSyncState);
        }

        public void Delete(ISyncState syncState)
        {
            var ddsSyncState = syncState as DdsSyncState;
            _store.Delete(ddsSyncState.Id);
        }
    }
}