namespace SyncItEasy.Core.Package
{
    public interface IState
    {
        string ProcessKey { get; set; }
        string Key { get; set; }
        string Hash { get; set; }
    }
}