namespace SyncWhatever.Tests.Fakes.Poco
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }

        public override string ToString()
        {
            return $"{Id} || {Name} || {RegistrationNumber}";
        }
    }
}