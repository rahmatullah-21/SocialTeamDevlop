namespace DominatorHouseCore.DatabaseHandler.Common.Accounts
{
    public interface IInteractedPosts : IPrimaryKey
    {
        string ActivityType { get; set; }
        int InteractionDate { get; set; }
    }
}
