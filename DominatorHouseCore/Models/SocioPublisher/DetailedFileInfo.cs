namespace DominatorHouseCore.Models.SocioPublisher
{
    public class DetailedFileInfo
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public DetailedFileInfo(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}