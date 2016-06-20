namespace SyncWhatever.Core.Package
{
    public interface ISyncKeyMap
    {
        string Context { get; set; }
        string Key { get; set; }
        string TargetKey { get; set; }
    }
}