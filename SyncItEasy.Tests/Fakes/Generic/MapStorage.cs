using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;

namespace SyncItEasy.Tests.Fakes
{
    public class MapStorage : IMapStorage
    {
        public List<ISyncMap> Storage = new List<ISyncMap>();

        public IEnumerable<ISyncMap> GetSyncMap()
        {
            return Storage;
        }

        public ISyncMap GetBySourceKey(string key)
        {
            return Storage.FirstOrDefault(x => x.SourceKey == key);
        }

        public void CreateOrUpdate(ISyncMap syncMap)
        {
            var existingState = Storage.FirstOrDefault(x => x.Id == syncMap.Id);

            if (existingState != null)
            {
                existingState.SourceKey = syncMap.SourceKey;
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