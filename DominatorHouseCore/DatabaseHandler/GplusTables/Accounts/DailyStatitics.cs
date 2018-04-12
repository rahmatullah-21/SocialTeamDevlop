using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class DailyStatitics
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Column(Order = 2)]
        public int Date
        { get; set; }

        [Column(Order = 3)]
        public int Followed
        { get; set; }

        [Column(Order = 4)]
        public int Unfollowed
        { get; set; }

        

        [Column(Order = 5)]
        public int LikesDone
        { get; set; }

        [Column(Order = 6)]
        public int CommentsDone
        { get; set; }

        [Column(Order = 7)]
        public int JoinedCommunities
        { get; set; }

        [Column(Order = 8)]
        public int UnjoinedCommunities
        { get; set; }

        [Column(Order = 9)]
        public int PeopleScraped
        { get; set; }

        [Column(Order = 10)]
        public int PostScraped
        { get; set; }

        [Column(Order = 11)]
        public int CommunityScraped
        { get; set; }

        //[Column(Order = 5)]
        //public int Uploads
        //{ get; set; }


    }
}
