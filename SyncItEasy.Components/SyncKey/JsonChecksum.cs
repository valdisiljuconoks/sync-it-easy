using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Components.SyncKey
{
    public class JsonChecksum : ISyncState
    {
        private JsonChecksum()
        {
        }

        public string Context { get; set; }
        public string Key { get; set; }
        public string Hash { get; set; }

        public static JsonChecksum Calculate(object value, object key)
        {
            var settings = new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            var json = JsonConvert.SerializeObject(value, Formatting.None, settings);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(jsonBytes);

            return new JsonChecksum
            {
                Key = key.ToString(),
                Hash = BitConverter.ToString(hashBytes)
            };
        }

        public override string ToString()
        {
            return $"Key = '{Key}', Hash = '{Hash}'";
        }
    }
}