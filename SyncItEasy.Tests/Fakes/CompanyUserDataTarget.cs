using SyncItEasy.Core.Package;
using SyncItEasy.Tests.Fakes.Poco;
using SyncItEasy.Tests.Fakes.REST;

namespace SyncItEasy.Tests.Fakes
{
    public class CompanyUserDataTarget : IDataTarget<Employee, CompanyUser>
    {
        private readonly int _companyId;

        public CompanyUserDataTarget(int companyId)
        {
            _companyId = companyId;
        }

        public string Insert(Employee source)
        {
            var target = new CompanyUser
            {
                CompanyId = _companyId,
                FirstName = source.Firstname,
                LastName = source.Lastname
            };
            target = CompanyUserApi.Put(_companyId, target);
            return target.Id.ToString();
        }

        public string Update(Employee source, CompanyUser target)
        {
            target.FirstName = source.Firstname;
            target.LastName = source.Lastname;
            target = CompanyUserApi.Post(_companyId, target.Id, target);
            return target.Id.ToString();
        }

        public void Delete(CompanyUser target)
        {
            CompanyUserApi.Delete(_companyId, target.Id);
        }

        public CompanyUser GetByKey(string key)
        {
            var id = int.Parse(key);
            return CompanyUserApi.Get(_companyId, id);
        }

        public string GetKeyBySourceItem(Employee sourceItem)
        {
            return null;
        }
    }
}