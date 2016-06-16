namespace SyncItEasy.Core.Package
{
    public interface IKeyMapStorage
    {
        ISyncMap GetBySourceKey(string partitionKey, string key);
        void CreateOrUpdate(ISyncMap syncMap);
        void Delete(ISyncMap map);
    }
}