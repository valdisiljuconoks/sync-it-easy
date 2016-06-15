using System.Collections.Generic;
using System.Linq;
using ConsoleApplication17_pak.Package;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests.Fakes
{
    public class CompanyUserStorage : IDataTarget<Employee, CompanyUser>
    {
        private readonly int _companyId;

        public CompanyUserStorage(int companyId)
        {
            _companyId = companyId;
        }

        private static int _idCounter = 1;

        public List<CompanyUser> Storage = new List<CompanyUser>();

        private CompanyUser RestGet(int companyId, int companyUserId)
        {
            return Storage
                .FirstOrDefault(x => x.CompanyId == companyId && x.Id == companyUserId);
        }
        private void RestAdd(int companyId, CompanyUser companyUser)
        {
            Storage.Add(companyUser);
        }
        private void RestUpdate(int companyId, int companyUserId, CompanyUser companyUser)
        {
            Storage.Add(companyUser);
        }
        private void RestDelete(int companyId, int companyUserId)
        {
            Storage.Remove(RestGet(companyId, companyUserId));
        }

        public string Insert(Employee source)
        {
            var companyUser = new CompanyUser
            {
                Id = _idCounter++,
                CompanyId = _companyId,
                FirstName = source.Firstname,
                LastName = source.Lastname,
            };
            RestAdd(_companyId, companyUser);
            return companyUser.Id.ToString();
        }

        public string Update(Employee source, CompanyUser target)
        {
            //target.CompanyId = ?
            target.FirstName = source.Firstname;
            target.LastName = source.Lastname;

            RestUpdate(_companyId, target.Id, target);
            return target.Id.ToString();
        }

        public void Delete(CompanyUser target)
        {
            RestDelete(_companyId, target.Id);
        }

        public CompanyUser GetByKey(string key)
        {
            var id = int.Parse(key);
            return RestGet(_companyId, id);
        }

        public CompanyUser GetByData(StateChange<Employee, CompanyUser> stateChange)
        {
            //TODO: add rules!
            return null;
        }
    }
}