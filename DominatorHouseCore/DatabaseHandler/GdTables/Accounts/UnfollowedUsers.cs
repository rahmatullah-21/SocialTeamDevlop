using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class UnfollowedUsers
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string FilterArgument
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int FilterTypeSql
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int FollowedBack
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int FollowedBackDate
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int InteractionDate
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public FollowType FollowType
        { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        [Unique]
        public string UnfollowedUsername
        { get; set; }
       
    }
}
