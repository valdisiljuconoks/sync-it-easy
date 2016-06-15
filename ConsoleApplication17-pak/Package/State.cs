namespace ConsoleApplication17_pak.Package
{
    public class State<TEntity> : IState
    {
        public string Entity => typeof(TEntity).FullName;
        public string Key { get; set; }
        public string Hash { get; set; }
    }
}