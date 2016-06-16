using System.Collections.Generic;
using System.Linq;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests.Fakes.REST
{
    public static class CompanyUserApi
    {
        private static int _idCounter = 1;


        public static List<CompanyUser> Storage = new List<CompanyUser>();

        public static CompanyUser Get(int companyId, int companyUserId)
        {
            return Storage
                .FirstOrDefault(x => x.CompanyId == companyId && x.Id == companyUserId);
        }

        public static CompanyUser Put(int companyId, CompanyUser companyUser)
        {
            if (companyUser.Id == 0)
            {
                companyUser.Id = _idCounter++;
            }

            Storage.Add(companyUser);

            return Get(companyId, companyUser.Id);
        }

        public static CompanyUser Post(int companyId, int companyUserId, CompanyUser companyUser)
        {
            var existing = Get(companyId, companyUserId);
            existing.FirstName = companyUser.FirstName;
            existing.LastName = companyUser.LastName;
            existing.Email = companyUser.Email;
            return existing;
        }

        public static void Delete(int companyId, int companyUserId)
        {
            Storage.Remove(Get(companyId, companyUserId));
        }
    }
}