namespace ConsoleApplication17_pak.Package
{
    internal class StateChange : IStateChange
    {
        public IState LastState { get; set; }
        public IState CurrentState { get; set; }
    }
}