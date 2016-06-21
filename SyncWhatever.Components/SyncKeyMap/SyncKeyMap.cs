using SyncWhatever.Core.Package;

namespace SyncWhatever.Components.SyncKeyMap
{
    public class SyncKeyMap : ISyncKeyMap
    {
        public string Context { get; set; }
        public string Key { get; set; }
        public string TargetKey { get; set; }

        public override string ToString()
        {
            return $"{Context} || {Key} || {TargetKey}";
        }
    }
}