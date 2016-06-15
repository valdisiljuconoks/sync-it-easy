namespace ConsoleApplication17_pak.Package
{
    public interface IStateStorage : IStateProvider
    {
        void CreateOrUpdate(IState syncState);
        void Delete(IState lastState);
    }
}