namespace SyncWhatever.Core.Package
{
    public interface ISyncState
    {
        string Context { get; set; }
        string Key { get; set; }
        string Hash { get; set; }
    }
}