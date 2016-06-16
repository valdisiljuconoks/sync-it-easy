namespace SyncItEasy.Core.Package
{
    public interface IDataTarget<TSource, TTarget> : ILookupByKey<TTarget>, ILookupBySourceItem<TSource, TTarget>,
        IEntityPersister<TSource, TTarget> where TSource : class where TTarget : class

    {
    }
}