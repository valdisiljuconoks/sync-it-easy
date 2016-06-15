using System;

namespace ConsoleApplication17_pak.Package
{
    public interface ISyncMap
    {
        Guid Id { get; set; }
        string AKey { get; set; }
        string BKey { get; set; }
    }
}