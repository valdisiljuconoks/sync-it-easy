namespace SyncItEasy.Core.Package
{
    public interface ISyncStateStore
    {
        void Create();
        void Update();
        void Delete();
    }
}