namespace ConsoleApplication17_pak.Package
{
    public interface IFallbackEntityProvider<TSource, TTarget> where TSource : class where TTarget : class
    {
        TTarget GetByData(StateChange<TSource, TTarget> stateChange);
    }
}