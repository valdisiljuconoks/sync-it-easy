using System;

namespace ConsoleApplication17_pak.Package
{
    public interface ISyncMap
    {
        Guid Id { get; set; }
        string SourceKey { get; set; }
        string TargetKey { get; set; }
    }
}