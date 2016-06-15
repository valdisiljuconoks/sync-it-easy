using System.Collections.Generic;

namespace ConsoleApplication17_pak.Package
{
    public interface IMapStorage
    {
        ISyncMap GetByKey(string key);
        //void Create(ISyncMap map);
        //void Update(ISyncMap map);
        //void Delete(ISyncMap map);
        void StoreSyncMap(ISyncMap aSyncMap);
    }
}