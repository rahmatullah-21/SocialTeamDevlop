using DominatorHouseCore.DatabaseHandler.Common;

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Accounts
{
    public class UnfollowedUsers : Entity
    {
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string AccountUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string UnfollowedUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string UnfollowedUserId  { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public FollowType FollowType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int Date { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Status { get; set; }

    }
}
