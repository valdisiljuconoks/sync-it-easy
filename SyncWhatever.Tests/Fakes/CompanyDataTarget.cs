using SyncWhatever.Core.Package;
using SyncWhatever.Tests.Fakes.Poco;
using SyncWhatever.Tests.Fakes.REST;

namespace SyncWhatever.Tests.Fakes
{
    public class CompanyDataTarget : IDataTarget<Organization, Company>
    {
        public string Insert(Organization source)
        {
            var company = new Company
            {
                RegistrationNumber = source.RegistrationNumber,
                Name = source.Name
            };
            company = CompanyApi.Put(company);
            return company.Id.ToString();
        }

        public string Update(Organization source, Company target)
        {
            target.RegistrationNumber = source.RegistrationNumber;
            target.Name = source.Name;
            target = CompanyApi.Post(target.Id, target);
            return target.Id.ToString();
        }

        public void Delete(Company target)
        {
            CompanyApi.Delete(target.Id);
        }

        public Company GetByKey(string key)
        {
            var id = int.Parse(key);
            return CompanyApi.Get(id);
        }

        public string GetKeyBySourceItem(Organization sourceItem)
        {
            return null;
            //return Storage
            //    .FirstOrDefault(x => x.RegistrationNumber == sourceItem.RegistrationNumber);
        }
    }
}