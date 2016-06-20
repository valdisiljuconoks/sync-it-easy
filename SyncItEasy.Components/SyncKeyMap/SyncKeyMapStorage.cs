using System.Collections.Generic;
using System.Linq;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Components.SyncKeyMap
{
    public class SyncKeyMapStorage : ISyncKeyMapStorage
    {
        public static List<ISyncKeyMap> Storage = new List<ISyncKeyMap>();

        public ISyncKeyMap GetBySourceKey(string context, string key)
        {
            return Storage.SingleOrDefault(x => x.Context == context && x.Key == key);
        }

        public void Create(string context, string key, string targetKey)
        {
            var syncKeyMap = new SyncKeyMap
            {
                Context = context,
                Key = key,
                TargetKey = targetKey
            };
            Storage.Add(syncKeyMap);
        }

        public void Update(ISyncKeyMap syncKeyMap)
        {
            // don't have to do anything as object is modified my reference already
        }

        public void Delete(ISyncKeyMap keyMap)
        {
            Storage.Remove(keyMap);
        }
    }
}