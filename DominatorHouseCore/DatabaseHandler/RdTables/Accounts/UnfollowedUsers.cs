using DominatorHouseCore.DatabaseHandler.Common.Accounts;
using System;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class UnfollowedUsers : BaseUnfollowedUsers
    {
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string FilterArgument
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int FilterTypeSql
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int FollowedBack
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int FollowedBackDate
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public override int InteractionDate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int OperationType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string FullName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public DateTime InteractionDateTime { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int InteractionTimeStamp { get; set; }
    }
}
