using System.Collections.Generic;

namespace SyncItEasy.Core.Package
{
    public interface IStoredStateProvider
    {
        IEnumerable<IState> GetStates(string processKey);
    }
}