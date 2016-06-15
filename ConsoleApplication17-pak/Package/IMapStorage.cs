using System.Collections.Generic;

namespace ConsoleApplication17_pak.Package
{
    public interface IMapStorage
    {
        ISyncMap GetBySourceKey(string key);
        void CreateOrUpdate(ISyncMap syncMap);
        void Delete(ISyncMap map);
    }
}