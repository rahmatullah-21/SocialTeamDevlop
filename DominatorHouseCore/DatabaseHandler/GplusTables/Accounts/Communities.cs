using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class Communities
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string Query { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        [Column(Order = 4)]
        public string ActivityType
        { get; set; }

        

        [Column(Order = 5)]
        public string CommunityId { get; set; }

        [Column(Order = 6)]
        public string CommunityUrl { get; set; }

        [Column(Order = 7)]
        public string CommunityName
        { get; set; }

        [Column(Order = 8)]
        public JoinType joinType
        { get; set; }

        [Column(Order = 9)]
        public int MemberCounts
        { get; set; }

        [Column(Order = 10)]
        public int InteractionDate { get; set; }

        [Column(Order = 11)]
        public int MuteStatus
        { get; set; }

        [Column(Order = 12)]
        public string OwnerName
        { get; set; }

        [Column(Order = 13)]
        public string OwnerId
        { get; set; }




        public enum JoinType : int
        {
            Joined, NotJoined, UnJoined
        }
    }
}
