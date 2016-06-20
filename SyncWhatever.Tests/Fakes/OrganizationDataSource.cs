using System.Collections.Generic;
using System.Linq;
using SyncWhatever.Components.SyncKey;
using SyncWhatever.Core.Package;
using SyncWhatever.Tests.Fakes.Poco;
using SyncWhatever.Tests.Fakes.Storage;

namespace SyncWhatever.Tests.Fakes
{
    public class OrganizationDataSource : IDataSource<Organization>
    {
        public IEnumerable<ISyncState> GetStates()
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