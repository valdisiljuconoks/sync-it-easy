using EPiServer.Data;
using EPiServer.Data.Dynamic;
using SyncItEasy.Core.Package;

namespace SyncItEasy.Components.EPiServer.SyncKeyMap
{
    [EPiServerDataStore(AutomaticallyRemapStore = true, AutomaticallyCreateStore = true,
        StoreName = nameof(DdsSyncKeyMapStorage))]
    [EPiServerDataContract]
    public class DdsSyncKeyMap : IDynamicData, ISyncKeyMap
    {
        // Workaround for the problem that DDS does not handle property names that matches SQL keywords: https://episerver.zendesk.com/hc/en-us/requests/38724
        [EPiServerDataMember]
        public string KeyValue { get; set; }

        [EPiServerDataMember]
        public Identity Id { get; set; }

        [EPiServerDataMember]
        public string Context { get; set; }

        // Do not map this property, see above
        public string Key
        {
            get { return KeyValue; }
            set { KeyValue = value; }
        }

        [EPiServerDataMember]
        public string TargetKey { get; set; }
    }
}