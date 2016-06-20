using System.Collections.Generic;
using System.Linq;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Components.SyncStateStorage
{
    public class SyncStateStorage : ISyncStateStorage
    {
        public static List<ISyncState> Storage = new List<ISyncState>();

        public IEnumerable<ISyncState> GetStates(string context)
        {
            return Storage.Where(x => x.Context == context);
        }

        public void Create(string context, string key, string hash)
        {
            var syncKeyMap = new SyncState
            {
                Context = context,
                Key = key,
                Hash = hash
            };
            Storage.Add(syncKeyMap);
        }

        public void Update(ISyncState syncState)
        {
            // don't have to do anything as object is modified my reference already
        }

        public void Delete(ISyncState lastSyncState)
        {
            Storage.Remove(lastSyncState);
        }
    }
}