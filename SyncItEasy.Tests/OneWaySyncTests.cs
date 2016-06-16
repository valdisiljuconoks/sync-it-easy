using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncItEasy.Core.Package;
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
        public ILog Log => LogManager.GetLogger("StressTest");


        [TestMethod]
        public void StressTest()
        {
            var organizationDataSource = new OrganizationDataSource();
            var companyDataTarget = new CompanyDataTarget();
            var stateStorage = new StateStorage();
            var keyMapStorage = new KeyMapStorage();

            var pk = 1;
            for (var times = 0; times < 10; times++)
            {
                Log.Debug($"PREPARING CHANGES");
                var random = new Random();
                for (var i = 0; i < 10; i++)
                {
                    var operation = random.Next(0, 5);

                    switch (operation)
                    {
                        case 0:
                        case 1:
                            //INSERT
                            var organization = new Organization
                            {
                                Id = pk++,
                                Name = Guid.NewGuid().ToString(),
                                RegistrationNumber = Guid.NewGuid().ToString()
                            };
                            OrganizationStorage.Storage.Add(organization);

                            Log.Debug($"Created organization '{organization.Id}'");
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

                Log.Debug($"DONE PREPARING CHANGES");

                if (OrganizationStorage.Storage.Count > 0)
                {
                    var orgNr = random.Next(0, OrganizationStorage.Storage.Count);

                    var organization = OrganizationStorage.Storage[orgNr];

                    for (var i = 0; i < 10; i++)
                    {
                        var operation = random.Next(0, 5);

                        switch (operation)
                        {
                            case 0:
                            case 1:
                                //INSERT
                                var employee = new Employee
                                {
                                    Id = pk++,
                                    Firstname = Guid.NewGuid().ToString(),
                                    Lastname = Guid.NewGuid().ToString()
                                };
                                organization.Employees.Add(employee);
                                Log.Debug($"Created employee '{employee.Id}' in organization '{organization.Id}'");

                                break;
                            case 2:
                            case 3:
                                //UPDATE
                                if (organization.Employees.Count > 0)
                                {
                                    var itemNr = random.Next(0, organization.Employees.Count);
                                    var update = organization.Employees[itemNr];
                                    update.Firstname = Guid.NewGuid().ToString();
                                }
                                break;

                            case 4:
                                //DELETE
                                if (organization.Employees.Count > 0)
                                {
                                    var itemNr = random.Next(0, organization.Employees.Count);
                                    organization.Employees.RemoveAt(itemNr);
                                }
                                break;
                        }
                    }
                }

                Action<string, string> nestedTasks = (sourceKey, targetKey) =>
                {
                    Log.Debug($"Sync for org {sourceKey}, t= '{targetKey}' started");


                    var organizationId = int.Parse(sourceKey);
                    var employeeStorage = new EmployeeDataSource(organizationId);

                    var companyId = int.Parse(targetKey);
                    var companyUserStorage = new CompanyUserDataTarget(companyId);

                    var employeeSyncTask = new SyncTask<Employee, CompanyUser>(
                        employeeStorage,
                        companyUserStorage,
                        stateStorage,
                        keyMapStorage,
                        null,
                        sourceKey);

                    employeeSyncTask.Execute();
                };

                var orgSyncTask = new SyncTask<Organization, Company>(
                    organizationDataSource,
                    companyDataTarget,
                    stateStorage,
                    keyMapStorage,
                    nestedTasks,
                    null
                    );

                orgSyncTask.Execute();


                try
                {
                    CheckCurrentSyncState();
                }
                catch (Exception ex)
                {
                    throw new Exception("State not valid!", ex);
                }
            }
            CheckCurrentSyncState();
        }

        private void CheckCurrentSyncState()
        {
            Assert.AreEqual(OrganizationStorage.Storage.Count, CompanyApi.Storage.Count);

            var expectedCount = OrganizationStorage.Storage.Count +
                                OrganizationStorage.Storage.SelectMany(x => x.Employees).Count();

            Assert.AreEqual(expectedCount, StateStorage.Storage.Count);
            Assert.AreEqual(expectedCount, KeyMapStorage.Storage.Count);

            foreach (var organization in OrganizationStorage.Storage)
            {
                var company =
                    CompanyApi.Storage.SingleOrDefault(x => x.RegistrationNumber == organization.RegistrationNumber);

                Assert.IsNotNull(company,
                    $"No matching company for reg nr '{organization.RegistrationNumber}' was found");

                var keyMap =
                    KeyMapStorage.Storage.SingleOrDefault(
                        x => x.Key == organization.Id.ToString() && x.TargetKey == company.Id.ToString());

                Assert.IsNotNull(keyMap, $"No matching keymap for reg nr '{organization.Id}' was found");


                var syncState =
                    StateStorage.Storage.SingleOrDefault(
                        x => x.ProcessKey == "Organization=>Company:root" && x.Key == organization.Id.ToString());

                Assert.IsNotNull(syncState, $"No matching sync state for org '{organization.Id}' was found");


                var companyUsers = CompanyUserApi.Storage.Where(x => x.CompanyId == company.Id);

                Assert.AreEqual(organization.Employees.Count, companyUsers.Count(),
                    $"Employee count differs for org '{organization.Id}' ");
            }
        }


        [TestMethod]
        public void BarTest()
        {
            var organizationDataSource = new OrganizationDataSource();
            var companyDataTarget = new CompanyDataTarget();
            var stateStorage = new StateStorage();
            var keyMapStorage = new KeyMapStorage();


            OrganizationStorage.Storage.Add(new Organization
            {
                Id = 1,
                Name = "kao",
                RegistrationNumber = "kaoreg",
                Employees = new List<Employee>
                {
                    new Employee
                    {
                        Id = 24,
                        Firstname = "Vilis",
                        Lastname = "Smits",
                        Email = "will@smith.com"
                    },
                    new Employee
                    {
                        Id = 224,
                        Firstname = "sVilis",
                        Lastname = "sSmits",
                        Email = "swill@smith.com"
                    }
                }
            });

            OrganizationStorage.Storage.Add(new Organization
            {
                Id = 2,
                Name = "kao2",
                RegistrationNumber = "kaoreg2",
                Employees = new List<Employee>
                {
                    new Employee
                    {
                        Id = 15,
                        Firstname = "Vilis2",
                        Lastname = "Smits2",
                        Email = "will@smith2.com"
                    }
                }
            });


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
                    null,
                    sourceKey
                    );

                employeeSyncTask.Execute();
            };

            var orgSyncTask = new SyncTask<Organization, Company>(
                organizationDataSource,
                companyDataTarget,
                stateStorage,
                keyMapStorage,
                nestedTasks,
                null
                );

            orgSyncTask.Execute();

            CheckCurrentSyncState();
        }
    }
}