using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class JobActivityManager
    {
        /// <summary>
        /// Module Configurations. FollowModule, UnfollowModule, LikeModule etc.
        /// </summary>
        [ProtoMember(1)]
        public List<ModuleConfiguration> LstModuleConfiguration { get; set; } = new List<ModuleConfiguration>();

        /// <summary>
        /// Day of week and Time when particular modules will be running
        /// </summary>
        [ProtoMember(2)]
        public List<RunningTimes> RunningTime { get; set; } = new List<RunningTimes>();


        public JobActivityManager()
        {
#if DEBUG            
            //FillConfigurations();
            //FillRunningTime();
#endif
        }

        private void FillRunningTime()
        {
            RunningTime = RunningTimes.DayWiseRunningTimes;
        }


        // TODO: have to be loaded from binary files
        void FillConfigurations()
        {            
            LstModuleConfiguration.Add(new ModuleConfiguration()
            {
                TemplateId = "",
                IsEnabled = true,         
                Status = "",
                LastUpdatedDate = DateTimeUtilities.GetEpochTime(),
                LstRunningTimes = new List<RunningTimes>(),
                ActivityType = ActivityType.Follow,
            });
        }


        #region Not Implemented Modules

        //[ProtoMember(2)]
        //public ModuleConfiguration UnfollowModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(3)]
        //public ModuleConfiguration LikeModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(4)]
        //public ModuleConfiguration UnlikeModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(5)]
        //public ModuleConfiguration CommentModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(6)]
        //public ModuleConfiguration DeleteCommentModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(7)]
        //public ModuleConfiguration PostingModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(8)]
        //public ModuleConfiguration RepostModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(9)]
        //public ModuleConfiguration DeletePostModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(10)]
        //public ModuleConfiguration MonitorFolderModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(11)]
        //public ModuleConfiguration MessageModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(12)]
        //public ModuleConfiguration UserScraperModule { get; set; } = new ModuleConfiguration();


        //[ProtoMember(13)]
        //public ModuleConfiguration PhotoScraperModule { get; set; } = new ModuleConfiguration();

        #endregion

    }
}