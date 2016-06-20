using System.Collections.Generic;

namespace SyncItEasy.Core.Package
{
    public interface IStateProvider
    {
        IEnumerable<ISyncState> GetStates();
    }
}