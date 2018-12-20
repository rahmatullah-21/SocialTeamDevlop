namespace DominatorHouseCore.DatabaseHandler.Common.Accounts
{

    public abstract class BaseUnfollowedUsers : Entity, IUnfollowedUser
    {

        public virtual int InteractionDate { get; set; }
    }
}
