using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IPostLikerViewModel
    {
        LikerCommentorConfigModel LikerCommentorConfigModel { get; set; }
        PostLikeCommentorModel PostLikeCommentorModel { get; set; }
    }
    public class PostLikerViewModel : StartupBaseViewModel, IPostLikerViewModel
    {
        public PostLikerViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.PostLiker });
            NextCommand = new DelegateCommand(PostLikerValidate);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;
            JobConfiguration = new JobConfiguration
            {

                ActivitiesPerJobDisplayName = "LangKeyNumberOfLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }


        private void PostLikerValidate()
        {
            if (!PostLikeCommentorModel.IsOwnWallChecked && !PostLikeCommentorModel.IsNewsfeedChecked &&
               !PostLikeCommentorModel.IsFriendTimeLineChecked && !PostLikeCommentorModel.IsCustomPostListChecked
               && !PostLikeCommentorModel.IsCampaignChecked && !PostLikeCommentorModel.IsGroupChecked
               && !PostLikeCommentorModel.IsPageChecked && !PostLikeCommentorModel.IsKeywordChecked
               && !PostLikeCommentorModel.IsCampaignChked)
            {
                Dialog.ShowDialog("Error", "Please Select atleast one option.");
                return;
            }

            if (PostLikeCommentorModel.IsFriendTimeLineChecked && PostLikeCommentorModel.ListFriendProfileUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query And Save.");
                return;
            }
            if (PostLikeCommentorModel.IsGroupChecked && PostLikeCommentorModel.ListGroupUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query And Save.");
                return;
            }

            if (PostLikeCommentorModel.IsPageChecked && PostLikeCommentorModel.ListPageUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query And Save.");
                return;
            }
            if (PostLikeCommentorModel.IsCampaignChecked && PostLikeCommentorModel.ListFaceDominatorCampaign.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query And Save.");
                return;
            }
            if (PostLikeCommentorModel.IsCustomPostListChecked && PostLikeCommentorModel.ListCustomPostList.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query And Save.");
                return;
            }
            if (PostLikeCommentorModel.IsKeywordChecked && PostLikeCommentorModel.ListKeywords.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query And Save.");
                return;
            }
            if (PostLikeCommentorModel.IsCampaignChked && PostLikeCommentorModel.ListCampaign.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query And Save.");
                return;
            }
            if (LikerCommentorConfigModel.ListReactionType.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one reaction type.");
                return;
            }
            if (IsActionasPageChecked && ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one own page.");
                return;
            }
            if (!IsActionasPageChecked && !IsActionasOwnAccountChecked)
            {
                Dialog.ShowDialog("Error", "Please select atleast one action as option.");
                return;
            }

            NevigateNext();
        }

        private PostLikeCommentorModel _postLikeCommentorModel = new PostLikeCommentorModel();
        public PostLikeCommentorModel PostLikeCommentorModel
        {
            get { return _postLikeCommentorModel; }
            set
            {
                if (_postLikeCommentorModel == value & _postLikeCommentorModel == null)
                    return;
                SetProperty(ref _postLikeCommentorModel, value);
            }
        }

        private LikerCommentorConfigModel _likerCommentorConfigModel = new LikerCommentorConfigModel();
        public LikerCommentorConfigModel LikerCommentorConfigModel
        {
            get { return _likerCommentorConfigModel; }
            set
            {
                if (_likerCommentorConfigModel == value & _likerCommentorConfigModel == null)
                    return;
                SetProperty(ref _likerCommentorConfigModel, value);
            }

        }

        private bool _isActionasOwnAccountChecked=true;
        public bool IsActionasOwnAccountChecked
        {
            get { return _isActionasOwnAccountChecked; }
            set
            {
                if (value == _isActionasOwnAccountChecked)
                    return;
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        private bool _isActionasPageChecked;
        public bool IsActionasPageChecked
        {
            get { return _isActionasPageChecked; }
            set
            {
                if (value == _isActionasPageChecked)
                    return;
                SetProperty(ref _isActionasPageChecked, value);
            }
        }

        private List<string> _listOwnPageUrl = new List<string>();
        public List<string> ListOwnPageUrl
        {
            get { return _listOwnPageUrl; }
            set
            {
                if (value == _listOwnPageUrl)
                    return;
                SetProperty(ref _listOwnPageUrl, value);
            }
        }

    }
}
