using System;

namespace ConsoleApplication17_pak.Package
{
    public interface ISyncMap
    {
        Guid Id { get; set; }
        string Key { get; set; }
        string TargetKey { get; set; }
    }
}