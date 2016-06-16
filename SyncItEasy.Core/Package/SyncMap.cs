using System;

namespace SyncItEasy.Core.Package
{
    public class SyncMap : ISyncMap
    {
        private SyncMap()
        {
        }

        public Guid Id { get; set; }
        public string ProcessKey { get; set; }
        public string Key { get; set; }
        public string TargetKey { get; set; }

        public static SyncMap Create(string processKey, string key)
        {
            var syncMap = new SyncMap
            {
                Id = Guid.NewGuid(),
                ProcessKey = processKey,
                Key = key
            };
            return syncMap;
        }
    }
}