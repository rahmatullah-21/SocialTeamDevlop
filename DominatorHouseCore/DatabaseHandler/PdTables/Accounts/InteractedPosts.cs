using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        //ID of the Pin
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string PinId { get; set; }

        //ID/Path of the media file
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string MediaString { get; set; }
        /// <summary>
        /// Message/Description of the Pin
        /// </summary>
        /// 

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string PinDescription { get; set; }

        //Like Count Of The Pin
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int TryCount { get; set; }

        //Comment Count Of The Pin
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int CommentCount { get; set; }


        //Time when the Pin has been posted in TimeStamp
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public double PinnedTimeStamp { get; set; }

        //Duration of the video Pins
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public double VideoDuration
        { get; set; }

        //ID of the Board
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string SourceBoard { get; set; }

        //Web url of the Pin
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string PinWebUrl { get; set; }

        // Board Name in which the Pin belongs to
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string SourceBoardName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int InteractionDate { get; set; }

        //Type of the Media(Image/Video)
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public MediaType MediaType { get; set; }

        //Type of Operation performed(follow/comment...etc)
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public ActivityType OperationType { get; set; }

        //User id of the User in which the Pin belongs to
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string UserId { get; set; }

        //Username of the User in which the Pin belongs to
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string QueryType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string CommentId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string BoardLabel { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string DestinationBoard { get; set; }

        public ActivityType GetActivityType()
        {
            return OperationType;
        }
    }
}
