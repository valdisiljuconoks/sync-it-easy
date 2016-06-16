using System.Collections.Generic;
using System.Linq;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Tests.Fakes.Generic
{
    public class KeyMapStorage : IKeyMapStorage
    {
        public static List<ISyncMap> Storage = new List<ISyncMap>();

        public ISyncMap GetBySourceKey(string partitionKey, string key)
        {
            return Storage.SingleOrDefault(x => x.ProcessKey == partitionKey && x.Key == key);
        }

        public void CreateOrUpdate(ISyncMap syncMap)
        {
            var existingState = Storage.SingleOrDefault(x => x.Key == syncMap.Key);

            if (existingState != null)
            {
                existingState.Key = syncMap.Key;
                existingState.TargetKey = syncMap.TargetKey;
            }
            else
            {
                Storage.Add(syncMap);
            }
        }

        public void Delete(ISyncMap map)
        {
            Storage.Remove(map);
        }
    }
}