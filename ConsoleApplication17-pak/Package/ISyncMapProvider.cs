using System.Collections.Generic;

namespace ConsoleApplication17_pak.Package
{
    public interface ISyncMapProvider
    {
        IEnumerable<ISyncMap> GetSyncMap();

        void StoreSyncMap(ISyncMap aSyncMap);
    }
}