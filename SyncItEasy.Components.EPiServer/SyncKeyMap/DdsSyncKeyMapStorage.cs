using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Components.EPiServer.SyncKeyMap
{
    public class DdsSyncKeyMapStorage : ISyncKeyMapStorage
    {
        private readonly DynamicDataStore _store;

        public DdsSyncKeyMapStorage()
        {
            _store = typeof(DdsSyncKeyMap).GetStore() ?? typeof(DdsSyncKeyMap).CreateStore();
        }

        public ISyncKeyMap GetBySourceKey(string context, string key)
        {
            return _store
                .Items<DdsSyncKeyMap>()
                .FirstOrDefault(x => x.Context == context && x.KeyValue == key);
        }

        public void Create(string context, string key, string targetKey)
        {
            var ddsSyncKeyMap = new DdsSyncKeyMap
            {
                Id = Identity.NewIdentity(),
                Context = context,
                KeyValue = key,
                TargetKey = targetKey
            };
            _store.Save(ddsSyncKeyMap);
        }

        public void Update(ISyncKeyMap syncKeyMap)
        {
            var ddsSyncKeyMap = syncKeyMap as DdsSyncKeyMap;
            _store.Save(ddsSyncKeyMap);
        }

        public void Delete(ISyncKeyMap syncKeyMap)
        {
            var ddsSyncKeyMap = syncKeyMap as DdsSyncKeyMap;

            if (ddsSyncKeyMap != null)
                _store.Delete(ddsSyncKeyMap.Id);
        }
    }
}