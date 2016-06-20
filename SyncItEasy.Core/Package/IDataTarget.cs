namespace SyncItEasy.Core.Package
{
    public interface IDataTarget<TSource, TTarget> : ILookupByKey<TTarget>, ILookupBySourceItem<TSource>,
        IEntityPersister<TSource, TTarget> where TSource : class where TTarget : class

    {
    }
}