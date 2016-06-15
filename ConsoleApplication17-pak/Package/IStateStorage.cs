namespace ConsoleApplication17_pak.Package
{
    public interface IStateStorage : IStateProvider
    {
        void SaveState(IState state);

    }
}