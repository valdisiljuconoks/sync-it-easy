using System;

namespace ConsoleApplication17_pak.Package
{
    public class SyncMap : ISyncMap
    {
        public Guid Id { get; set; }
        public string AKey { get; set; }
        public string BKey { get; set; }
    }
}