namespace SyncItEasy.Core.Package
{
    public interface ISyncStateStorage : IStoredStateProvider
    {
        void Create(string context, string key, string hash);
        void Update(ISyncState syncState);
        void Delete(ISyncState lastSyncState);
    }
}