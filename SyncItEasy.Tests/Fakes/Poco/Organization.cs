﻿using System;
using System.Collections.Generic;

namespace SyncItEasy.Tests.Fakes.Poco
{
    [Serializable]
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public List<Employee> Employees { get; set; }

    }
}