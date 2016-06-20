using System.Collections.Generic;

namespace SyncWhatever.Core.Package
{
    public interface IStoredStateProvider
    {
        IEnumerable<ISyncState> GetStates(string context);
    }
}