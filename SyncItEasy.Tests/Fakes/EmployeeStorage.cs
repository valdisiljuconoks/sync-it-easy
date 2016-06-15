using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;
using SyncItEasy.Tests.Fakes.Generic;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests.Fakes
{
    public class EmployeeStorage : IEntityProvider<Employee>, ICurrentStateProvider
    {

        public List<Employee> Employees { get; set; }



        public IEnumerable<IState> GetStates()
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