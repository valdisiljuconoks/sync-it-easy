using System;
using System.Linq;
using ConsoleApplication17_pak.Package;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncItEasy.Tests.Fakes;
using SyncItEasy.Tests.Fakes.Generic;
using SyncItEasy.Tests.Fakes.Poco;

namespace SyncItEasy.Tests
{
    [TestClass]
    public class OneWaySyncTests
    {
        [TestMethod]
        public void StressTest()
        {
            var organizationStorage = new OrganizationStorage();
            var companyStorage = new CompanyStorage();
            var stateStorage = new StateStorage();
            var mapStorage = new KeyMapStorage();

            var pk = 0;
            for (var times = 0; times < 100; times++)
            {
                var random = new Random();
                for (var i = 0; i < 10; i++)
                {
                    var operation = random.Next(0, 5);

                    if (times > 10)
                    {
                        break;
                    }

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
                            organizationStorage.Storage.Add(user);
                            break;
                        case 2:
                        case 3:
                            //UPDATE
                            if (organizationStorage.Storage.Count > 0)
                            {
                                var itemNr = random.Next(0, organizationStorage.Storage.Count);
                                var update = organizationStorage.Storage[itemNr];
                                update.Name = Guid.NewGuid().ToString();
                            }
                            break;

                        case 4:
                            //DELETE
                            if (organizationStorage.Storage.Count > 0)
                            {
                                var itemNr = random.Next(0, organizationStorage.Storage.Count);
                                organizationStorage.Storage.RemoveAt(itemNr);
                            }
                            break;
                    }
                }

                Action<string, string> nestedTasks = (aKey, bKey) =>
                {
                    //var companyId = int.Parse(bKey);

                    //var empLastState = new LastStateProvider<Employee>();
                    //var syncMapProvider = new SyncMapProvider();

                    //var employeeStorage = new EmployeeStorage
                    //{
                    //    Employees = sourceStorage.GetByKey(aKey).Employees
                    //};


                    //var companyUserStorage = new CompanyUserStorage(companyId);


                    //   var employeeSyncTask = new SyncTask<Employee, CompanyUser>(
                    //        employeeStorage,
                    //        empLastState,
                    //        employeeStorage, 
                    //        companyUserStorage,
                    //        null,
                    //        companyUserStorage,
                    //        syncMapProvider,
                    //        null
                    //);

                    //   employeeSyncTask.Execute();
                };

                var orgSyncTask = new SyncTask<Organization, Company>(
                    organizationStorage,
                    companyStorage,
                    stateStorage,
                    mapStorage
                    );

                orgSyncTask.Execute();
            }

            Assert.AreEqual(organizationStorage.Storage.Count, companyStorage.Storage.Count);
            Assert.AreEqual(organizationStorage.Storage.Count, stateStorage.Storage.Count);
            Assert.AreEqual(organizationStorage.Storage.Count, mapStorage.Storage.Count);

            foreach (var source in organizationStorage.Storage)
            {
                var employee =
                    companyStorage.Storage.FirstOrDefault(x => x.RegistrationNumber == source.RegistrationNumber);

                Assert.IsNotNull(employee);
            }
        }
    }
}