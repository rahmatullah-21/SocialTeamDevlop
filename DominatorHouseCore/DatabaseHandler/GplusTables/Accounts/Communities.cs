using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class Communities
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType
        { get; set; }

        

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string CommunityId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string CommunityUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string CommunityName
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public JoinType joinType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int MemberCounts
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int MuteStatus
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string OwnerName
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string OwnerId
        { get; set; }




        public enum JoinType : int
        {
            Joined, NotJoined, UnJoined
        }
    }
}
