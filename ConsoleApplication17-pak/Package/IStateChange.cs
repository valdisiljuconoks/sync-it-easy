namespace ConsoleApplication17_pak.Package
{
    internal interface IStateChange
    {
        IState LastState { get; set; }
        IState CurrentState { get; set; }
    }
}