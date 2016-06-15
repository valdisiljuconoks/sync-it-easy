namespace ConsoleApplication17_pak.Package
{
    public interface IDataSource<out TSource> : IStateProvider, IEntityProvider<TSource> where TSource : class
    {
    }
}