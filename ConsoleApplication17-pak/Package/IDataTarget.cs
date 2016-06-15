namespace ConsoleApplication17_pak.Package
{
    public interface IDataTarget<TSource, TTarget> : IEntityProvider<TTarget>, IFallbackEntityProvider<TSource, TTarget>, IEntityPersister<TSource, TTarget> where TSource : class where TTarget : class

    {

    }
}