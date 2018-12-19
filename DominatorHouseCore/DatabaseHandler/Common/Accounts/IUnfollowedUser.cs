namespace DominatorHouseCore.DatabaseHandler.Common.Accounts
{
    public interface IUnfollowedUser : IPrimaryKey
    {
        int InteractionDate { get; set; }
    }
}
