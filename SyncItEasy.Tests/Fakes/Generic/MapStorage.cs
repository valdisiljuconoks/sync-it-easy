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

        public ISyncMap GetByKey(string key)
        {
            return Storage.FirstOrDefault(x => x.SourceKey == key);
        }

        public void StoreSyncMap(ISyncMap syncMap)
        {
            if (syncMap.Id == Guid.Empty)
                syncMap.Id = Guid.NewGuid();

            var existingState = Storage.FirstOrDefault(x => x.Id == syncMap.Id);

            if (existingState != null)
            {
                if (syncMap.SourceKey == null || syncMap.TargetKey == null)
                {
                    Storage.Remove(existingState);
                }
                else
                {
                    existingState.SourceKey = syncMap.SourceKey;
                    existingState.TargetKey = syncMap.TargetKey;
                }
            }
            else
            {
                Storage.Add(syncMap);
            }
        }
    }
}