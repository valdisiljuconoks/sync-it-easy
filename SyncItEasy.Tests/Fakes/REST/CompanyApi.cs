using System.Collections.Generic;
using System.Linq;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests.Fakes.REST
{
    public static class CompanyApi
    {
        private static int _idCounter = 1;


        public static List<Company> Storage = new List<Company>();

        public static Company Get(int companyId)
        {
            return Storage
                .FirstOrDefault(x => x.Id == companyId);
        }

        public static Company Put(Company company)
        {
            if (company.Id == 0)
            {
                company.Id = _idCounter++;
            }

            Storage.Add(company);

            return Get(company.Id);
        }

        public static Company Post(int companyId, Company company)
        {
            Storage.Add(company);
            return Get(companyId);
        }

        public static void Delete(int companyId)
        {
            Storage.Remove(Get(companyId));
        }
    }
}