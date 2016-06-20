using SyncItEasy.Core.Package;

namespace SyncItEasy.Components.SyncKeyMap
{
    public class SyncKeyMap : ISyncKeyMap
    {
        public string Context { get; set; }
        public string Key { get; set; }
        public string TargetKey { get; set; }
    }
}