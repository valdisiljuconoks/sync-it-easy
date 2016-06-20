namespace SyncWhatever.Core.Package
{
    public interface ILookupBySourceItem<TSource> where TSource : class
    {
        string GetKeyBySourceItem(TSource source);
    }
}