namespace SyncItEasy.Core.Package
{
    public interface IStateStorage : IStoredStateProvider
    {
        void CreateOrUpdate(IState syncState);
        void Delete(IState lastState);
    }
}