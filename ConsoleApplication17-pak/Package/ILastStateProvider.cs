namespace ConsoleApplication17_pak.Package
{
    public interface ILastStateProvider : IStateProvider
    {
        void SaveState(IState state);

    }
}