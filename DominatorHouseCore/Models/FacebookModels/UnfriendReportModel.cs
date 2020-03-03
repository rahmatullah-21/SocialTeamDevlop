using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models.FacebookModels
{
    
    public class UnfriendReportModel : BindableBase
    {


        public int Id { get; set; }


        public string AccountEmail { get; set; } = string.Empty;


        public string QueryType { get; set; } = string.Empty;


        public string QueryValue { get; set; } = string.Empty;



        public string ActivityType
        { get; set; } = string.Empty;


        public string UserId
        { get; set; } = string.Empty;

        private string _userProfilePic = string.Empty;
        public string UserProfilePicUrl
        {
            get
            {
                return _userProfilePic;
            }
            set
            {
                SetProperty(ref _userProfilePic, value);
            }
        } 



        public string UserName
        { get; set; } = string.Empty;

        public string DetailedUserInfo
        { get; set; } = string.Empty;

        /*
                public DateTime ConnectionDate { get; set; }
        */

        public DateTime InteractionTimeStamp { get; set; }


        public string Message
        { get; set; } = string.Empty;


        public string UploadedMediaPath
        { get; set; } = string.Empty;
        public string PublishedPostUrl { get; set; }
        public bool IsPublishedPostOnTimeline { get; set; }
        public string PostDescription { get; set; }
    }
}
