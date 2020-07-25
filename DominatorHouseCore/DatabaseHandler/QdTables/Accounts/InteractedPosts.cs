using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public MediaType MediaType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public ActivityType ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string PkOwner { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int TakenAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string UsernameOwner { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Comment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string CommentId { get; set; }

        public ActivityType GetActivityType()
        {
            return ActivityType;

        }
    }
}
