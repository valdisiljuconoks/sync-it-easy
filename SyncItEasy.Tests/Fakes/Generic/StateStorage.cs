using System.Collections.Generic;
using System.Linq;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Tests.Fakes.Generic
{
    public class StateStorage : IStateStorage
    {
        public static List<IState> Storage = new List<IState>();

        public IEnumerable<IState> GetStates(string processKey)
        {
            return Storage.Where(x => x.ProcessKey == processKey);
        }

        public void CreateOrUpdate(IState syncState)
        {
            var existingState =
                Storage.SingleOrDefault(x => x.ProcessKey == syncState.ProcessKey && x.Key == syncState.Key);

            if (existingState != null)
            {
                existingState.Hash = syncState.Hash;
            }
            else
            {
                Storage.Add(syncState);
            }
        }

        public void Delete(IState lastState)
        {
            Storage.Remove(lastState);
        }
    }
}