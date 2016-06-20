using System.Collections.Generic;

namespace SyncItEasy.Core.Package
{
    public interface IStoredStateProvider
    {
        IEnumerable<ISyncState> GetStates(string context);
    }
}