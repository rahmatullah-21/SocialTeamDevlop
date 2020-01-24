using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class InteractedUsersForUnfriend
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Contains PostId For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string PostId { get; set; }

        /// <summary>
        /// Describes Activity like cmment like or share
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string ActivityType { get; set; }

        /// <summary>
        /// Contains FullName Of the Interacted User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string UserId { get; set; }

        /// <summary>
        /// Contains TimeStamp when interacted with the User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int InteractionTimeStamp { get; set; }

        /// <summary>
        /// Contains Date time
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public DateTime InteractionDateTime { get; set; }

    }
}
