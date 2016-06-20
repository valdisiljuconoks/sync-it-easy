using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Components.SyncKey
{
    public class BinaryChecksum : ISyncState
    {
        private BinaryChecksum()
        {
        }

        public string Context { get; set; }
        public string Key { get; set; }
        public string Hash { get; set; }

        public static BinaryChecksum Calculate(object value, object key)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                var objectBytes = stream.ToArray();

                var md5 = MD5.Create();
                var hashBytes = md5.ComputeHash(objectBytes);

                return new BinaryChecksum
                {
                    Key = key.ToString(),
                    Hash = BitConverter.ToString(hashBytes)
                };
            }
        }

        public override string ToString()
        {
            return $"Key = '{Key}', Hash = '{Hash}'";
        }
    }
}