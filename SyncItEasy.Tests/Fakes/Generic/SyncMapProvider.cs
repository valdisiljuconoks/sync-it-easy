using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;

namespace SyncItEasy.Tests.Fakes
{
    public class SyncMapProvider : ISyncMapProvider
    {
        public List<ISyncMap> Storage = new List<ISyncMap>();

        public IEnumerable<ISyncMap> GetSyncMap()
        {
            return Storage;
        }

        public void StoreSyncMap(ISyncMap syncMap)
        {
            if (syncMap.Id == Guid.Empty)
                syncMap.Id = Guid.NewGuid();

            var existingState = Storage.FirstOrDefault(x => x.Id == syncMap.Id);

            if (existingState != null)
            {
                if (syncMap.AKey == null || syncMap.BKey == null)
                {
                    Storage.Remove(existingState);
                }
                else
                {
                    existingState.AKey = syncMap.AKey;
                    existingState.BKey = syncMap.BKey;
                }
            }
            else
            {
                Storage.Add(syncMap);
            }
        }
    }
}