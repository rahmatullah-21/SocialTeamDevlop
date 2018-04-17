using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class UnfollowedUsers
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string AccountUsername
        { get; set; }

        [Column(Order = 3)]
        public string FilterArgument
        { get; set; }


        [Column(Order = 4)]
        public int FilterTypeSql
        { get; set; }


        [Column(Order = 5)]
        public int FollowedBack
        { get; set; }


        [Column(Order = 6)]
        public int FollowedBackDate
        { get; set; }


        [Column(Order = 7)]
        public int InteractionDate
        { get; set; }


        [Column(Order = 8)]
        public FollowType FollowType
        { get; set; }


        [Column(Order = 9)]
        [Unique]
        public string UnfollowedUsername
        { get; set; }
       
    }
}
