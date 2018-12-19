using SQLite;

namespace DominatorHouseCore.DatabaseHandler.Common.Accounts
{

    public abstract class BaseUnfollowedUsers : IUnfollowedUser
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        public virtual int InteractionDate { get; set; }
    }
}
