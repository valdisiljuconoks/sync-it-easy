namespace ConsoleApplication17_pak.Package
{
    public interface IFallbackEntityProvider<out TEntity, in TSource> where TEntity : class where TSource: class
    {
        TEntity GetBySourceEntity(TSource source);
    }
}