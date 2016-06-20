namespace SyncWhatever.Core.Package
{
    public interface ILookupByKey<out TEntity> where TEntity : class
    {
        TEntity GetByKey(string key);
    }
}