namespace ConsoleApplication17_pak.Package
{
    public interface IEntityPersister<in TEntity, in TSource>
    {
        string Insert(TSource source);
        string Update(TSource source, TEntity target);
        void Delete(TEntity target);

    }

   
}