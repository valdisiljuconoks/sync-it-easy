using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;
using SyncItEasy.Tests.Fakes.Generic;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests.Fakes
{
    public class OrganizationStorage : IEntityProvider<Organization>, ICurrentStateProvider
    {
        public List<Organization> Storage = new List<Organization>();

        public IEnumerable<IState> GetStates()
        {
            return Storage
                .Select(x => BinaryChecksum.Calculate(x, x.Id));
        }

        public Organization GetByKey(string key)
        {
            var id = int.Parse(key);
            return Storage
                .FirstOrDefault(x => x.Id == id);
        }
    }
}