using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;
using SyncItEasy.Tests.Fakes.Generic;
using SyncItEasy.Tests.Fakes.Poco;
using SyncItEasy.Tests.Fakes.Storage;

namespace SyncItEasy.Tests.Fakes
{
    public class EmployeeDataSource : IDataSource<Employee>
    {
        private readonly int _organizationId;

        public EmployeeDataSource(int organizationId)
        {
            _organizationId = organizationId;
        }


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

        public IEnumerable<Employee> Employees
            => OrganizationStorage.Storage.Where(x => x.Id == _organizationId).SelectMany(x => x.Employees);
    }
}