using System;

namespace ConsoleApplication17_pak.Package
{
    public class SyncMap : ISyncMap
    {
        public Guid Id { get; set; }
        public string SourceKey { get; set; }
        public string TargetKey { get; set; }
    }
}