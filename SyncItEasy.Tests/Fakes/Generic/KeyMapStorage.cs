using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;

namespace SyncItEasy.Tests.Fakes.Generic
{
    public class KeyMapStorage : IKeyMapStorage
    {
        public List<ISyncMap> Storage = new List<ISyncMap>();

        public ISyncMap GetBySourceKey(string key)
        {
            return Storage.FirstOrDefault(x => x.Key == key);
        }

        public void CreateOrUpdate(ISyncMap syncMap)
        {
            var existingState = Storage.FirstOrDefault(x => x.Key == syncMap.Key);

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