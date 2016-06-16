namespace SyncItEasy.Core.Package
{
    public class State : IState
    {
        public string ProcessKey { get; set; }
        public string Key { get; set; }
        public string Hash { get; set; }

        public override string ToString()
        {
            return $"{ProcessKey}:{Key} => {Hash}";
        }
    }
}