namespace ConsoleApplication17_pak.Package
{
    public interface ILookupBySourceItem<TSource, TTarget> where TSource : class where TTarget : class
    {
        TTarget GetBySourceItem(TSource source);
    }
}