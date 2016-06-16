namespace SyncItEasy.Core.Package
{
    public class StateChange<TSource, TTarget>
    {
        public string SourceKey { get; set; }
        public string TargetKey { get; set; }
        public ISyncMap SyncMap { get; set; }
        public IState LastState { get; set; }
        public IState CurrentState { get; set; }
        public TSource SourceItem { get; set; }
        public TTarget TargetItem { get; set; }
        public OperationEnum Operation { get; set; }
    }
}