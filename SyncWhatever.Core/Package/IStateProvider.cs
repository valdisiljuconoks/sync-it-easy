using System.Collections.Generic;

namespace SyncWhatever.Core.Package
{
    public interface IStateProvider
    {
        IEnumerable<ISyncState> GetStates();
    }
}