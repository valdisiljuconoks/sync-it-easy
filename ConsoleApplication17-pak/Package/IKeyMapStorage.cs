using System.Collections.Generic;

namespace ConsoleApplication17_pak.Package
{
    public interface IKeyMapStorage
    {
        ISyncMap GetBySourceKey(string key);
        void CreateOrUpdate(ISyncMap syncMap);
        void Delete(ISyncMap map);
    }
}