using System.Collections.Generic;
using System.Linq;
using SyncItEasy.Core.Package;
using SyncItEasy.Tests.Fakes.Generic;
using SyncItEasy.Tests.Fakes.Poco;
using SyncItEasy.Tests.Fakes.Storage;

namespace SyncItEasy.Tests.Fakes
{
    public class OrganizationDataSource : IDataSource<Organization>
    {
        public IEnumerable<IState> GetStates()
        {
            return OrganizationStorage.Storage
                .Select(x => BinaryChecksum.Calculate(x, x.Id));
        }

        public Organization GetByKey(string key)
        {
            var id = int.Parse(key);
            return OrganizationStorage.Storage
                .FirstOrDefault(x => x.Id == id);
        }
    }
}