namespace DominatorHouseCore.Interfaces
{
    public interface IChannel
    {
        string ChannelId { get; set; }

        string Channelname { get; set; }

        string FullName { get; set; }

        string ProfilePicUrl { get; set; }
    }
}