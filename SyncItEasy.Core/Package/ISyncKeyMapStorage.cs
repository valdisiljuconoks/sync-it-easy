namespace SyncItEasy.Core.Package
{
    public interface ISyncKeyMapStorage
    {
        ISyncKeyMap GetBySourceKey(string context, string key);
        void Create(string context, string key, string targetKey);
        void Update(ISyncKeyMap syncKeyMap);
        void Delete(ISyncKeyMap keyMap);
    }
}