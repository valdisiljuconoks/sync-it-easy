using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncItEasy.Components.SyncKeyMap;
using SyncItEasy.Components.SyncStateStorage;
using SyncItEasy.Core.Package;
using SyncItEasy.Tests.Fakes;
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
            var stateStorage = new SyncStateStorage();
            var keyMapStorage = new SyncKeyMapStorage();

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
                                Log.Debug($"Updated organization '{update.Id}'");
                            }
                            break;

                        case 4:
                            //DELETE
                            if (OrganizationStorage.Storage.Count > 0)
                            {
                                var itemNr = random.Next(0, OrganizationStorage.Storage.Count);
                                OrganizationStorage.Storage.RemoveAt(itemNr);
                                Log.Debug($"Deleted organization");
                            }
                            break;
                    }
                }

                

                if (OrganizationStorage.Storage.Count > 0&&false)
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

                Log.Debug($"DONE PREPARING CHANGES");

                Action<string, string, Organization, Company> nestedTasks = (sourceKey, targetKey, sourceItem, targetItem) =>
                {
                    Log.Debug($"Sync for org {sourceKey}, t= '{targetKey}' started");


                    var organizationId = int.Parse(sourceKey);
                    var employeeStorage = new EmployeeDataSource(organizationId);

                    var companyId = int.Parse(targetKey);
                    var companyUserStorage = new CompanyUserDataTarget(companyId);

                    var employeeSyncTask = new SyncTask<Employee, CompanyUser>(
                        "EmployeeSyncTask",
                        employeeStorage,
                        companyUserStorage,
                        stateStorage,
                        keyMapStorage,
                        null,
                        sourceKey);

                    employeeSyncTask.Execute();
                };

                var orgSyncTask = new SyncTask<Organization, Company>(
                    "OrganizationSyncTask",
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
        }

        private void CheckCurrentSyncState()
        {
            Assert.AreEqual(OrganizationStorage.Storage.Count, CompanyApi.Storage.Count);

            var expectedCount = OrganizationStorage.Storage.Count +
                                OrganizationStorage.Storage.SelectMany(x => x.Employees).Count();

            Assert.AreEqual(expectedCount, SyncStateStorage.Storage.Count);
            Assert.AreEqual(expectedCount, SyncKeyMapStorage.Storage.Count);

            foreach (var organization in OrganizationStorage.Storage)
            {
                var company =
                    CompanyApi.Storage.SingleOrDefault(x => x.RegistrationNumber == organization.RegistrationNumber);

                Assert.IsNotNull(company,
                    $"No matching company for reg nr '{organization.RegistrationNumber}' was found");

                var keyMap =
                    SyncKeyMapStorage.Storage.SingleOrDefault(
                        x => x.Key == organization.Id.ToString() && x.TargetKey == company.Id.ToString());

                Assert.IsNotNull(keyMap, $"No matching keymap for reg nr '{organization.Id}' was found");


                var syncState =
                    SyncStateStorage.Storage.SingleOrDefault(
                        x => x.Context == "[OrganizationSyncTask]:[]" && x.Key == organization.Id.ToString());

                Assert.IsNotNull(syncState, $"No matching sync state for org '{organization.Id}' was found");


                var companyUsers = CompanyUserApi.Storage.Where(x => x.CompanyId == company.Id);

                Assert.AreEqual(organization.Employees.Count, companyUsers.Count(),
                    $"Employee count differs for org '{organization.Id}' ");
            }
        }

    }
}