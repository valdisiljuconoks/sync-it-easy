using System;

namespace SyncItEasy.Core.Package
{
    public interface ISyncMap
    {
        Guid Id { get; set; }
        string ProcessKey { get; set; }
        string Key { get; set; }
        string TargetKey { get; set; }
    }
}