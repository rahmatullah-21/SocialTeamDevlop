using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
   public class GeneralModel : BindableBase
    {
        private int _noOfPost;
      
        [ProtoMember(1)]
        public int NoOfPost
        {
            get
            {
                return _noOfPost;
            }
            set
            {
                if (_noOfPost == value)
                    return;
                SetProperty(ref _noOfPost, value);
            }
        }
        private int _waitMaxOf;

        [ProtoMember(2)]
        public int WaitMaxOf
        {
            get
            {
                return _waitMaxOf;
            }
            set
            {
                if (_waitMaxOf == value)
                    return;
                SetProperty(ref _waitMaxOf, value);
            }
        }
        private int _keepMaxOf;

        [ProtoMember(3)]
        public int KeepMaxOf
        {
            get
            {
                return _keepMaxOf;
            }
            set
            {
                if (_keepMaxOf == value)
                    return;
                SetProperty(ref _keepMaxOf, value);
            }
        }
        private int _checkRSSFeedsminutes;

        [ProtoMember(4)]
        public int CheckRssFeedsminutes
        {
            get
            {
                return _checkRSSFeedsminutes;
            }
            set
            {
                if (_checkRSSFeedsminutes == value)
                    return;
                SetProperty(ref _checkRSSFeedsminutes, value);
            }
        }
        private int _checkMonitorFoldersminutes;

        [ProtoMember(5)]
        public int CheckMonitorFoldersminutes
        {
            get
            {
                return _checkMonitorFoldersminutes;
            }
            set
            {
                if (_checkMonitorFoldersminutes == value)
                    return;
                SetProperty(ref _checkMonitorFoldersminutes, value);
            }
        }
        private bool _isAutomaticallyRetryChecked;

        [ProtoMember(6)]
        public bool IsAutomaticallyRetryChecked
        {
            get
            {
                return _isAutomaticallyRetryChecked;
            }
            set
            {
                if (_isAutomaticallyRetryChecked == value)
                    return;
                SetProperty(ref _isAutomaticallyRetryChecked, value);
            }
        }
        private bool _isChooseSingleRandomImageChecked;

        [ProtoMember(7)]
        public bool IsChooseSingleRandomImageChecked
        {
            get
            {
                return _isChooseSingleRandomImageChecked;
            }
            set
            {
                if (_isChooseSingleRandomImageChecked == value)
                    return;
                SetProperty(ref _isChooseSingleRandomImageChecked, value);
            }
        }
        private bool _isChooseOnlyFirstImageChecked;

        [ProtoMember(8)]
        public bool IsChooseOnlyFirstImageChecked
        {
            get
            {
                return _isChooseOnlyFirstImageChecked;
            }
            set
            {
                if (_isChooseOnlyFirstImageChecked == value)
                    return;
                SetProperty(ref _isChooseOnlyFirstImageChecked, value);
            }
        }
        private bool _isChooseBetweenChecked;

        [ProtoMember(9)]
        public bool IsChooseBetweenChecked
        {
            get
            {
                return _isChooseBetweenChecked;
            }
            set
            {
                if (_isChooseBetweenChecked == value)
                    return;
                SetProperty(ref _isChooseBetweenChecked, value);
            }
        }
        private RangeUtilities _chooseBetween=new RangeUtilities();

        [ProtoMember(10)]
        public RangeUtilities ChooseBetween
        {
            get
            {
                return _chooseBetween;
            }
            set
            {
                if (_chooseBetween == value)
                    return;
                SetProperty(ref _chooseBetween, value);
            }
        }
        private bool _isWhenPublishingSendOnePostChecked;

        [ProtoMember(11)]
        public bool IsWhenPublishingSendOnePostChecked
        {
            get
            {
                return _isWhenPublishingSendOnePostChecked;
            }
            set
            {
                if (_isWhenPublishingSendOnePostChecked == value)
                    return;
                SetProperty(ref _isWhenPublishingSendOnePostChecked, value);
            }
        }
        private bool _isChooseRandomPostsChecked;

        [ProtoMember(12)]
        public bool IsChooseRandomPostsChecked
        {
            get
            {
                return _isChooseRandomPostsChecked;
            }
            set
            {
                if (_isChooseRandomPostsChecked == value)
                    return;
                SetProperty(ref _isChooseRandomPostsChecked, value);
            }
        }
        private bool _isInsertPostsAtRandomChecked;

        [ProtoMember(13)]
        public bool IsInsertPostsAtRandomChecked
        {
            get
            {
                return _isInsertPostsAtRandomChecked;
            }
            set
            {
                if (_isInsertPostsAtRandomChecked == value)
                    return;
                SetProperty(ref _isInsertPostsAtRandomChecked, value);
            }
        }
        private bool _isDoNotPublishPostsChecked;

        [ProtoMember(14)]
        public bool IsDoNotPublishPostsChecked
        {
            get
            {
                return _isDoNotPublishPostsChecked;
            }
            set
            {
                if (_isDoNotPublishPostsChecked == value)
                    return;
                SetProperty(ref _isDoNotPublishPostsChecked, value);
            }
        }
        private bool _isRemoveLinkFromPostsChecked;

        [ProtoMember(15)]
        public bool IsRemoveLinkFromPostsChecked
        {
            get
            {
                return _isRemoveLinkFromPostsChecked;
            }
            set
            {
                if (_isRemoveLinkFromPostsChecked == value)
                    return;
                SetProperty(ref _isRemoveLinkFromPostsChecked, value);
            }
        }
    }
}
