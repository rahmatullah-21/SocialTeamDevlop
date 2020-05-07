using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.Models.FacebookModels
{
    [ProtoContract]
    public class LikerCommentorConfigModel : BindableBase
    {
        private bool _isLikeTypeFilterChkd = true;

        [ProtoMember(1)]
        public bool IsLikeTypeFilterChkd
        {
            get
            {
                return _isLikeTypeFilterChkd;
            }
            set
            {
                SetProperty(ref _isLikeTypeFilterChkd, value);
            }
        }




        private bool _isHahaFilterChkd;

        [ProtoMember(2)]
        // ReSharper disable once IdentifierTypo
        public bool IsHahaFilterChkd
        {
            get
            {
                return _isHahaFilterChkd;
            }
            set
            {
                SetProperty(ref _isHahaFilterChkd, value);
            }
        }



        private bool _isLikeFilterChkd = true;

        [ProtoMember(3)]
        // ReSharper disable once IdentifierTypo
        public bool IsLikeFilterChkd
        {
            get
            {
                return _isLikeFilterChkd;
            }
            set
            {
                SetProperty(ref _isLikeFilterChkd, value);
            }
        }

        private bool _isLoveFilterChkd;

        [ProtoMember(4)]
        // ReSharper disable once UnusedMember.Global
        public bool IsLoveFilterChkd
        {
            get
            {
                return _isLoveFilterChkd;
            }
            set
            {
                SetProperty(ref _isLoveFilterChkd, value);
            }
        }

        private bool _isWowFilterChkd;

        [ProtoMember(5)]
        // ReSharper disable once IdentifierTypo
        public bool IsWowFilterChkd
        {
            get
            {
                return _isWowFilterChkd;
            }
            set
            {
                SetProperty(ref _isWowFilterChkd, value);
            }
        }

        private bool _isSadFilterChkd;

        [ProtoMember(6)]
        // ReSharper disable once UnusedMember.Global
        public bool IsSadFilterChkd
        {
            get
            {
                return _isSadFilterChkd;
            }
            set
            {
                SetProperty(ref _isSadFilterChkd, value);
            }
        }

        private bool _isAngryFilterChkd;

        [ProtoMember(7)]
        // ReSharper disable once UnusedMember.Global
        public bool IsAngryFilterChkd
        {
            get
            {
                return _isAngryFilterChkd;
            }
            set
            {
                SetProperty(ref _isAngryFilterChkd, value);
            }
        }

        private bool _isCareFilterChkd;

        [ProtoMember(8)]
        public bool IsCareFilterChkd
        {
            get
            {
                return _isCareFilterChkd;
            }
            set
            {
                SetProperty(ref _isCareFilterChkd, value);
            }
        }

        private bool _isCommentFilterChecked = true;

        [ProtoMember(9)]
        public bool IsCommentFilterChecked
        {
            get
            {
                return _isCommentFilterChecked;
            }
            set
            {
                SetProperty(ref _isCommentFilterChecked, value);
            }
        }


        private ObservableCollectionBase<ManageCustomCommentsModel> _savedComments = new ObservableCollectionBase<ManageCustomCommentsModel>();

        [ProtoMember(10)]
        public ObservableCollectionBase<ManageCustomCommentsModel> SavedComments
        {
            get { return _savedComments; }
            set
            {
                SetProperty(ref _savedComments, value);
            }
        }

        private ManageCustomCommentsModel _currentCommment = new ManageCustomCommentsModel();

        [ProtoMember(11)]
        public ManageCustomCommentsModel CurrentCommment
        {
            get { return _currentCommment; }
            set
            {
                SetProperty(ref _currentCommment, value);
            }
        }


        [ProtoMember(12)]
        public ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();


        [ProtoMember(13)]

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        private List<ReactionType> _lstReactionType = new List<ReactionType>();

        [ProtoMember(14)]

        public List<ReactionType> ListReactionType
        {
            get { return _lstReactionType; }
            set
            {
                SetProperty(ref _lstReactionType, value);
            }
        }

        private bool _isSpintaxChecked;

        [ProtoMember(15)]
        public bool IsSpintaxChecked
        {
            get { return _isSpintaxChecked; }
            set
            {
                SetProperty(ref _isSpintaxChecked, value);
            }
        }



    }
}
