namespace DominatorHouseCore.DatabaseHandler.Common.Accounts
{
    public interface IInteractedUsers : IPrimaryKey
    {
        int InteractionDate { get; set; }
    }
}
