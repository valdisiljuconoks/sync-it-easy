using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests.Fakes
{
    public class CompanyStorage : IDataTarget<Organization, Company>
    {
        private static int _idCounter = 1;

        public List<Company> Storage = new List<Company>();

        public IEnumerable<IState> GetStates()
        {
            return Storage
                .Select(x => new State<CompanyUser> {Key = x.Id.ToString(), Hash = x.Name});
        }

        public string Insert(Organization source)
        {
            var company = new Company
            {
                Id = _idCounter++,
                RegistrationNumber = source.RegistrationNumber,
                Name = source.Name
            };
            Storage.Add(company);
            return company.Id.ToString();
        }

        public string Update(Organization source, Company target)
        {
            target.RegistrationNumber = source.RegistrationNumber;
            target.Name = source.Name;
            return target.Id.ToString();
        }

        public void Delete(Company target)
        {
            Storage.Remove(target);
        }

        public Company GetByKey(string key)
        {
            var id = int.Parse(key);
            return Storage
                .FirstOrDefault(x => x.Id == id);
        }

        public Company GetBySourceEntity(Organization source)
        {
            return Storage.FirstOrDefault(x => x.RegistrationNumber == source.RegistrationNumber);
        }
    }
}