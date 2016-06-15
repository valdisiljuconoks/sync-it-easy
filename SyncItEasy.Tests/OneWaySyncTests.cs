using System;
using System.Linq;
using ConsoleApplication17_pak.Package;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncItEasy.Tests.Fakes;
using SyncItEasy.Tests.Fakes.Generic;
using SyncItEasy.Tests.Fakes.Poco;
using SyncItEasy.Tests.Fakes.REST;
using SyncItEasy.Tests.Fakes.Storage;

namespace SyncItEasy.Tests
{
    [TestClass]
    public class OneWaySyncTests
    {
        [TestMethod]
        public void StressTest()
        {
            var organizationDataSource = new OrganizationDataSource();
            var companyDataTarget = new CompanyDataTarget();
            var stateStorage = new StateStorage();
            var keyMapStorage = new KeyMapStorage();

            var pk = 1;
            for (var times = 0; times < 100; times++)
            {
                var random = new Random();
                for (var i = 0; i < 10; i++)
                {
                    var operation = random.Next(0, 5);


                    switch (operation)
                    {
                        case 0:
                        case 1:
                            //INSERT
                            var user = new Organization
                            {
                                Id = pk++,
                                Name = Guid.NewGuid().ToString(),
                                RegistrationNumber = Guid.NewGuid().ToString()
                            };
                            OrganizationStorage.Storage.Add(user);
                            break;
                        case 2:
                        case 3:
                            //UPDATE
                            if (OrganizationStorage.Storage.Count > 0)
                            {
                                var itemNr = random.Next(0, OrganizationStorage.Storage.Count);
                                var update = OrganizationStorage.Storage[itemNr];
                                update.Name = Guid.NewGuid().ToString();
                            }
                            break;

                        case 4:
                            //DELETE
                            if (OrganizationStorage.Storage.Count > 0)
                            {
                                var itemNr = random.Next(0, OrganizationStorage.Storage.Count);
                                OrganizationStorage.Storage.RemoveAt(itemNr);
                            }
                            break;
                    }
                }

                Action<string, string> nestedTasks = (sourceKey, targetKey) =>
                {
                    var organizationId = int.Parse(sourceKey);
                    var employeeStorage = new EmployeeDataSource(organizationId);

                    var companyId = int.Parse(targetKey);
                    var companyUserStorage = new CompanyUserDataTarget(companyId);

                    var employeeSyncTask = new SyncTask<Employee, CompanyUser>(
                        employeeStorage,
                        companyUserStorage,
                        stateStorage,
                        keyMapStorage,
                        null);

                    //employeeSyncTask.Execute();
                };

                var orgSyncTask = new SyncTask<Organization, Company>(
                    organizationDataSource,
                    companyDataTarget,
                    stateStorage,
                    keyMapStorage,
                    nestedTasks
                    );

                orgSyncTask.Execute();
            }

            Assert.AreEqual(OrganizationStorage.Storage.Count, CompanyApi.Storage.Count);
            Assert.AreEqual(OrganizationStorage.Storage.Count, stateStorage.Storage.Count);
            Assert.AreEqual(OrganizationStorage.Storage.Count, keyMapStorage.Storage.Count);

            foreach (var organization in OrganizationStorage.Storage)
            {
                var company = CompanyApi.Storage.SingleOrDefault(x => x.RegistrationNumber == organization.RegistrationNumber);

                Assert.IsNotNull(company);

                var keyMap =
                    keyMapStorage.Storage.SingleOrDefault(
                        x => x.Key == organization.Id.ToString() && x.TargetKey == company.Id.ToString());

                Assert.IsNotNull(keyMap);


                var syncState =
                   stateStorage.Storage.SingleOrDefault(
                       x => x.Key == organization.Id.ToString());

                Assert.IsNotNull(syncState);
            }
        }
    }
}