namespace ConsoleApplication17_pak.Package
{
    public interface IState
    {
        string Entity { get; }
        string Key { get; set; }
        string Hash { get; set; }
    }
}