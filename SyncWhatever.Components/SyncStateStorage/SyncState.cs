using SyncWhatever.Core.Package;

namespace SyncWhatever.Components.SyncStateStorage
{
    public class SyncState : IStoredSyncState
    {
        public string Context { get; set; }
        public string Key { get; set; }
        public string Hash { get; set; }

        public override string ToString()
        {
            return $"{Context} || {Key} || {Hash}";
        }
    }
}