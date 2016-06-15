namespace ConsoleApplication17_pak.Package
{
    public interface IDataSource<out TSource> : IStateProvider, ILookupByKey<TSource> where TSource : class
    {
    }
}