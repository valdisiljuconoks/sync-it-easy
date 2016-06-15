using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;

namespace SyncItEasy.Tests.Fakes.Generic
{
    public class StateStorage : IStateStorage
    {
        public List<IState> Storage = new List<IState>();

        public IEnumerable<IState> GetStates(string partition = null)
        {
            return Storage;
        }
        
        public void CreateOrUpdate(IState syncState)
        {
            var existingState = Storage.FirstOrDefault(x => x.Key == syncState.Key);

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