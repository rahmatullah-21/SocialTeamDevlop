using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using Newtonsoft.Json;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Interfaces;

namespace DominatorHouseCore.BusinessLogic
{
    public partial class DominatorScheduler
    {
        public  static Action<int,int> ChangeTabIndex { get; set; }


        /// <summary>
        /// ScheduleForEachModule take two argument first is Module type  and second is an object of AccountModel
        /// it will schedule job for all module having running time except given moduleType if moduleType is 
        /// null it will schedule for all module having running time
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="account"></param>
        public static void ScheduleForEachModule(ActivityType? moduleType, DominatorAccountModel account)
        {
            try
            {
                foreach (ActivityType at in Enum.GetValues(typeof(ActivityType)))
                {
                    if (at != moduleType)
                    {
                        var moduleRunningTimes = GetRunningTimes(account, at);
                        if (moduleRunningTimes.Count() > 0)
                        {
                            account.ActivityManager.RunningTime = moduleRunningTimes;
                            account.ActivityManager.RunningTime.ForEach(timing =>
                            {
                                foreach (var timingRange in timing.Timings)
                                {
                                    timingRange.Module = at.ToString();
                                }
                            });

                            DominatorScheduler.ScheduleTodayJobs(account, at);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.ToString());
            }
        }
        /// <summary>
        /// GetRunningTimes take two argument first is an object of AccountModel and second is Module type
        /// it will return running time list according to Module type
        /// </summary>
        /// <param name="item"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static List<RunningTimes> GetRunningTimes(DominatorAccountModel item, ActivityType moduleType)            
        {
            var activitySetting = string.Empty;
            var runningTime = new List<RunningTimes>();
            try
            {
                var moduleConfiguration = item.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == moduleType);
                activitySetting = BinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == moduleConfiguration.TemplateId).ActivitySettings;

                runningTime = JsonConvert.DeserializeObject<IGeneralSettings>(activitySetting).JobConfiguration.RunningTime;
                
                #region Commented
                //switch (moduleType)
                //{
                //    case ActivityType.Follow:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.FollowModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<FollowerModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Unfollow:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.UnfollowModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<UnfollowerModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Like:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.LikeModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<LikeModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Comment:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.CommentModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<CommentModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Repost:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.RepostModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<RePosterModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.DownloadScraper:
                //        break;
                //    case ActivityType.UserScraper:
                //        break;
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug(ex); 
            }

            return runningTime;
        }


        public static bool CompareRunningTime(List<RunningTimes> firstRunningTime, List<RunningTimes> secondRunningTime)
        {

            if ((firstRunningTime == null && secondRunningTime != null) || (secondRunningTime == null && firstRunningTime != null))
                return false;
            else if (firstRunningTime == null && secondRunningTime == null)
                return true;

            if (firstRunningTime.Count != secondRunningTime.Count)
                return false;

            bool IsEqual = true;
            foreach (RunningTimes item in firstRunningTime)
            {
                RunningTimes oldRunningTime = item;
                RunningTimes newRunningTime = secondRunningTime.ElementAt(firstRunningTime.IndexOf(item));


                if ((oldRunningTime == null && newRunningTime != null) || (newRunningTime == null && oldRunningTime != null))
                {
                    IsEqual = false;
                    break;
                }
                if (oldRunningTime.Timings.Count != newRunningTime.Timings.Count)
                    return false;
                foreach (var oldtime in oldRunningTime.Timings)
                {
                    if (!newRunningTime.Timings.Contains(oldtime))
                        return false;
                    IsEqual = true;
                }

            }

            return IsEqual;
        }
    }




}
