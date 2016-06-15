namespace ConsoleApplication17_pak.Package
{
    public interface IEntityProvider<out TEntity> where TEntity : class
    {
        TEntity GetByKey(string key);
    }
}