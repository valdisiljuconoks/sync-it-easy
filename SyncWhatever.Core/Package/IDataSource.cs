namespace SyncWhatever.Core.Package
{
    public interface IDataSource<out TSource> : IStateProvider, ILookupByKey<TSource> where TSource : class
    {
    }
}