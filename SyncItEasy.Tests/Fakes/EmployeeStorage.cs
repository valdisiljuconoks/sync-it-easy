using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;
using SyncItEasy.Tests.Fakes.Generic;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests.Fakes
{
    public class EmployeeStorage : IDataSource<Employee>
    {

        public List<Employee> Employees { get; set; }

        public IEnumerable<IState> GetStates(string partition = null)
        {
            return Employees
                .Select(x => BinaryChecksum.Calculate(x, x.Id));
        }

        public Employee GetByKey(string key)
        {
            var id = int.Parse(key);
            return Employees
                .FirstOrDefault(x => x.Id == id);
        }
    }
}