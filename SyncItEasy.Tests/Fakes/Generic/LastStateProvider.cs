using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;

namespace SyncItEasy.Tests.Fakes
{
    public class LastStateProvider<T> : ILastStateProvider
    {
        public List<IState> Storage = new List<IState>();

        public IEnumerable<IState> GetStates()
        {
            return Storage;
        }
        public void SaveState(IState state)
        {
            var existingState = Storage.FirstOrDefault(x => x.Key == state.Key);

            if (existingState != null)
            {
                if (state.Hash == null)
                    Storage.Remove(existingState);
                else
                    existingState.Hash = state.Hash;
            }
            else
            {
                Storage.Add(state);
            }
        }

    }
}