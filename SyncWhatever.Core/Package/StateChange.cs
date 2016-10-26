namespace SyncWhatever.Core.Package
{
    public class StateChange<TSource, TTarget>
    {
        public string SourceKey { get; set; }
        public string TargetKey { get; set; }
        public ISyncKeyMap SyncKeyMap { get; set; }
        public ISyncState LastSyncState { get; set; }
        public ISyncState CurrentSyncState { get; set; }
        public TSource SourceItem { get; set; }
        public TTarget TargetItem { get; set; }
        public OperationEnum Operation { get; set; }

        public override string ToString()
        {
            return $"{nameof(SourceKey)}: {SourceKey}, {nameof(TargetKey)}: {TargetKey}, {nameof(SyncKeyMap)}: {SyncKeyMap}, {nameof(LastSyncState)}: {LastSyncState}, {nameof(CurrentSyncState)}: {CurrentSyncState}, {nameof(SourceItem)}: {SourceItem}, {nameof(TargetItem)}: {TargetItem}, {nameof(Operation)}: {Operation}";
        }
    }
}
