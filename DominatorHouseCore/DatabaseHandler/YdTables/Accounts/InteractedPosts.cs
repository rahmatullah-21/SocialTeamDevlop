using DominatorHouseCore.Enums;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
{
    public class InteractedPosts
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }
        [Column(Order = 4)]
        public string QueryValue { get; set; }

        [Column(Order = 5)]
        public string ActivityType { get; set; }

        [Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }
        
        [Column(Order = 7)]
        public string VideoId { get; set; }
        
        [Column(Order = 8)]
        public string UserId { get; set; }

        [Column(Order = 9)]
        public string Username { get; set; }
        
        [Column(Order = 10)]
        public string ChannelId { get; set; }

        [Column(Order = 11)]
        public string ChannelUrl { get; set; }

        [Column(Order = 12)]
        public string PostTitle { get; set; }

        [Column(Order = 13)]
        public DateTime PostUrl { get; set; }

        [Column(Order = 14)]
        public int LikeCount { get; set; }

        [Column(Order = 15)]
        public int DisLikeCount { get; set; }
        
        [Column(Order = 16)]
        public int CommentCount { get; set; }
        
        [Column(Order = 17)]
        public int PostedTimeStamp { get; set; }
        
        [Column(Order = 18)]
        public string VideoDuration
        { get; set; }
        
        [Column(Order = 19)]
        public int ViewCount
        { get; set; }

        [Column(Order = 20)]
        public List<string> ListCommentedText { get; set; }

        [Column(Order = 21)]
        public List<string> UpNextVideos { get; set; }
        

        [Column(Order = 22)]
        public string PostDescription
        { get; set; }

        
        [Column(Order = 23)]
        public string PostCategory
        { get; set; }

        [Column(Order = 24)]
        public int SubscribeCount
        { get; set; }

        [Column(Order = 25)]
        public bool IsAlreadySubscribed
        { get; set; }

        [Column(Order = 26)]
        public bool IsSubscribed
        { get; set; }

        [Column(Order = 27)]
        public bool IsAlreadyLiked
        { get; set; }


        [Column(Order = 28)]
        public bool IsAlreadyDisLiked
        { get; set; }

        [Column(Order = 29)]
        public bool IsLiked
        { get; set; }


        [Column(Order = 30)]
        public bool IsDisLiked
        { get; set; }

       
        [Column(Order = 31)]
        public string ProcessType { get; set; }

        [Column(Order = 32)]
        public DateTime InteractionDate { get; set; }

        [Column(Order = 33)]
        public string Query { get; set; }

        [Column(Order = 34)]
        public string ChannelTitle { get; set; }

    }
}
