using System;

namespace SyncItEasy.Tests.Fakes.Poco
{
    [Serializable]

    public class Employee
    {
        public int Id { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }

    }
}