using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using ConsoleApplication17_pak.Package;

namespace SyncItEasy.Tests.Fakes.Generic
{
    public class BinaryChecksum : IState
    {
        private BinaryChecksum()
        {
        }


        public string Entity { get; set; }
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
                    Entity = value.GetType().FullName,
                    Key = key.ToString(),
                    Hash = BitConverter.ToString(hashBytes)
                };
            }
        }
    }
}