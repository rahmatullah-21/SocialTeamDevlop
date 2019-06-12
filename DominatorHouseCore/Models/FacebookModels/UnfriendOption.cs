using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using DominatorHouseCore;

namespace DominatorHouseCore.Models.FacebookModels
{
    public interface IUnfriendOption
    {
        /*bool IsAddedThroughSoftware { get; set; }

        bool IsAddedOutsideSoftware { get; set; }

        bool CustomUserList { get; set; }

        bool IsAlreadyFriends { get; set; }

        bool IsPendingRequest { get; set; }

        bool IsFilterApplied { get; set; }

        int DaysBefore { get; set; }

        int HoursBefore { get; set; }*/

        int Count { get; set; }

        int TypeCount { get; set; }

//        string BySoftwareDisplayName { get; set; }
//
//        string OutsideSoftwareDisplayName { get; set; }

        string FilterText { get; set; }

        string SourceDisplayName { get; set; }


//        List<string> LstFilterText { get; set; }
//
//        bool IsFilterByMessageText { get; set; }
//
//        bool IsCustomUserList { get; set; }
//
//        string CustomUserText { get; set; }
//
//        List<string> LstCustomUsers { get; set; }
//
//        bool IsMutualFriends { get; set; }

    }

    public class UnfriendOption : BindableBase, IUnfriendOption
    {
        private bool _isAddedThroughSoftware;

        [ProtoMember(1)]
        public bool IsAddedThroughSoftware
        {
            get
            {
                return _isAddedThroughSoftware;
            }
            set
            {
                if (value == _isAddedThroughSoftware)
                    return;
                SetProperty(ref _isAddedThroughSoftware, value);

            }
        }

        private bool _isAddedOutsideSoftware;

        [ProtoMember(2)]
        public bool IsAddedOutsideSoftware
        {
            get
            {
                return _isAddedOutsideSoftware;
            }
            set
            {
                if (value == _isAddedOutsideSoftware)
                    return;
                SetProperty(ref _isAddedOutsideSoftware, value);

            }
        }

        
        /*private bool _customUserList;

        [ProtoMember(3)]
        public bool CustomUserList
        {
            get
            {
                return _customUserList;
            }
            set
            {
                if (value == _customUserList)
                    return;
                SetProperty(ref _customUserList, value);

            }
        }

        private bool _isAlreadyFriends;

        [ProtoMember(4)]
        public bool IsAlreadyFriends
        {
            get
            {
                return _isAlreadyFriends;
            }

            set
            {
                if (value == _isAlreadyFriends)
                    return;
                SetProperty(ref _isAlreadyFriends, value);

            }
        }

        private bool _isPendingRequest;

        [ProtoMember(5)]
        public bool IsPendingRequest
        {
            get
            {
                return _isPendingRequest;
            }

            set
            {
                if (value == _isPendingRequest)
                    return;
                SetProperty(ref _isPendingRequest, value);

            }
        }*/

        private bool _isFilterApplied;

        [ProtoMember(6)]
        public bool IsFilterApplied
        {
            get
            {
                return _isFilterApplied;
            }

            set
            {
                if (value == _isFilterApplied)
                    return;
                SetProperty(ref _isFilterApplied, value);

            }
        }

        private int _daysBefore;

        [ProtoMember(7)]
        public int DaysBefore
        {
            get
            {
                return _daysBefore;
            }

            set
            {
                if (value == _daysBefore)
                    return;
                SetProperty(ref _daysBefore, value);

            }
        }


        private int _hoursBefore; 

        [ProtoMember(8)]
        public int HoursBefore
        {
            get
            {
                return _hoursBefore;
            }

            set
            {
                if (value == _hoursBefore)
                    return;
                SetProperty(ref _hoursBefore, value);

            }
        }

        private int _count;

        [ProtoMember(9)]
        public int Count
        {
            get
            {
                return _count;
            }

            set
            {
                if (value == _count)
                    return;
                SetProperty(ref _count, value);

            }
        }

        int _typeCount;

        [ProtoMember(10)]
        public int TypeCount
        {
            get
            {
                return _typeCount;
            }

            set
            {
                if (value == _typeCount)
                    return;
                SetProperty(ref _typeCount, value);

            }
        }


        private string _bySoftwareDisplayName = string.Empty;

        [ProtoMember(11)]
        public string BySoftwareDisplayName
        {
            get
            {
                return _bySoftwareDisplayName;
            }

            set
            {
                if (value == _bySoftwareDisplayName)
                    return;
                SetProperty(ref _bySoftwareDisplayName, value);

            }
        }




        string _outsideSoftwareDisplayName = string.Empty;

        [ProtoMember(12)]
        public string OutsideSoftwareDisplayName
        {
            get
            {
                return _outsideSoftwareDisplayName;
            }

            set
            {
                if (value == _outsideSoftwareDisplayName)
                    return;
                SetProperty(ref _outsideSoftwareDisplayName, value);

            }
        }


        private string _filterText = string.Empty;

        [ProtoMember(13)]
        public string FilterText
        {
            get
            {
                return _filterText;
            }

            set
            {
                if (value == _filterText)
                    return;
                SetProperty(ref _filterText, value);

            }
        }

        private List<string> _lstFilterText = new List<string>();

        [ProtoMember(14)]
        public List<string> LstFilterText
        {
            get
            {
                return _lstFilterText;
            }

            set
            {
                if (value == _lstFilterText)
                    return;
                SetProperty(ref _lstFilterText, value);

            }
        }

        /*private bool _isFilterByMessageText ;

        [ProtoMember(15)]
        public bool IsFilterByMessageText
        {
            get
            {
                return _isFilterByMessageText;
            }

            set
            {
                if (value == _isFilterByMessageText)
                    return;
                SetProperty(ref _isFilterByMessageText, value);

            }
        }*/

        private string _sourceDisplayName;

        [ProtoMember(16)]
        public string SourceDisplayName
        {
            get
            {
                return _sourceDisplayName;
            }

            set
            {
                if (value == _sourceDisplayName)
                    return;
                SetProperty(ref _sourceDisplayName, value);

            }
        }




        private string _customUserText;

        [ProtoMember(17)]
        public string CustomUserText
        {
            get
            {
                return _customUserText;
            }

            set
            {
                if (value == _customUserText)
                    return;
                SetProperty(ref _customUserText, value);

            }
        }


        private List<string> _lstCustomUsers;

        [ProtoMember(18)]
        public List<string> LstCustomUsers
        {
            get
            {
                return _lstCustomUsers;
            }

            set
            {
                if (value == _lstCustomUsers)
                    return;
                SetProperty(ref _lstCustomUsers, value);

            }
        }

        private bool _isCustomUserList;

        [ProtoMember(19)]
        public bool IsCustomUserList
        {
            get
            {
                return _isCustomUserList;
            }

            set
            {
                if (value == _isCustomUserList)
                    return;
                SetProperty(ref _isCustomUserList, value);

            }
        }

        private bool _isMutualFriends;

        [ProtoMember(6)]
        public bool IsMutualFriends
        {
            get
            {
                return _isMutualFriends;
            }

            set
            {
                if (value == _isMutualFriends)
                    return;
                SetProperty(ref _isMutualFriends, value);

            }
        }

    }



    public interface IAutoReplyOptionModel
    {
        /*bool IsFriendsMessageChecked { get; set; }

        bool IsMessageRequestChecked { get; set; }

        bool CustomUserList { get; set; }

        bool IsAlreadyFriends { get; set; }

        bool IsPendingRequest { get; set; }

        bool IsFilterApplied { get; set; }

        int DaysBefore { get; set; }

        int HoursBefore { get; set; }*/

        int Count { get; set; }

        int TypeCount { get; set; }

        /*string BySoftwareDisplayName { get; set; }

        string OutsideSoftwareDisplayName { get; set; }

        string FilterText { get; set; }

        string SourceDisplayName { get; set; }

        List<string> LstFilterText { get; set; }

        bool IsFilterByIncommingMessageText { get; set; }

        bool IsFilterByMessageRequestText { get; set; }*/

        bool IsReplyToPageMessagesChecked { get; set; }

        string OwnPages { get; set; }

    }

    public class AutoReplyOptionModel : BindableBase, IAutoReplyOptionModel
    {
        private bool _isFriendsMessageChecked;

        [ProtoMember(1)]
        public bool IsFriendsMessageChecked
        {
            get
            {
                return _isFriendsMessageChecked;
            }
            set
            {
                if (value == _isFriendsMessageChecked)
                    return;
                ChangeCount(value);
                SetProperty(ref _isFriendsMessageChecked, value);

            }
        }

     

        private bool _isAddedOutsideSoftware;

        [ProtoMember(2)]
        public bool IsMessageRequestChecked
        {
            get
            {
                return _isAddedOutsideSoftware;
            }
            set
            {
                if (value == _isAddedOutsideSoftware)
                    return;
                ChangeCount(value);
                SetProperty(ref _isAddedOutsideSoftware, value);

            }
        }


        /*private bool _customUserList;

        [ProtoMember(3)]
        public bool CustomUserList
        {
            get
            {
                return _customUserList;
            }
            set
            {
                if (value == _customUserList)
                    return;
                ChangeCount(value);
                SetProperty(ref _customUserList, value);

            }
        }*/

        /*private bool _isAlreadyFriends;

        [ProtoMember(4)]
        public bool IsAlreadyFriends
        {
            get
            {
                return _isAlreadyFriends;
            }

            set
            {
                if (value == _isAlreadyFriends)
                    return;
                ChangeTypeCount(value);
                SetProperty(ref _isAlreadyFriends, value);

            }
        }

        private bool _isPendingRequest;

        [ProtoMember(5)]
        public bool IsPendingRequest
        {
            get
            {
                return _isPendingRequest;
            }

            set
            {
                if (value == _isPendingRequest)
                    return;
                ChangeTypeCount(value);
                SetProperty(ref _isPendingRequest, value);

            }
        }*/

        private bool _isFilterApplied;

        [ProtoMember(6)]
        public bool IsFilterApplied
        {
            get
            {
                return _isFilterApplied;
            }

            set
            {
                if (value == _isFilterApplied)
                    return;
                SetProperty(ref _isFilterApplied, value);

            }
        }

        private int _daysBefore;

        [ProtoMember(7)]
        public int DaysBefore
        {
            get
            {
                return _daysBefore;
            }

            set
            {
                if (value == _daysBefore)
                    return;
                SetProperty(ref _daysBefore, value);

            }
        }


        private int _hoursBefore;

        [ProtoMember(8)]
        public int HoursBefore
        {
            get
            {
                return _hoursBefore;
            }

            set
            {
                if (value == _hoursBefore)
                    return;
                SetProperty(ref _hoursBefore, value);

            }
        }

        private int _count;

        [ProtoMember(9)]
        public int Count
        {
            get
            {
                return _count;
            }

            set
            {
                if (value == _count)
                    return;
                SetProperty(ref _count, value);

            }
        }

        int _typeCount;

        [ProtoMember(10)]
        public int TypeCount
        {
            get
            {
                return _typeCount;
            }

            set
            {
                if (value == _typeCount)
                    return;
                SetProperty(ref _typeCount, value);

            }
        }


        private string _bySoftwareDisplayName = string.Empty;

        [ProtoMember(11)]
        public string BySoftwareDisplayName
        {
            get
            {
                return _bySoftwareDisplayName;
            }

            set
            {
                if (value == _bySoftwareDisplayName)
                    return;
                SetProperty(ref _bySoftwareDisplayName, value);

            }
        }



        private string _outsideSoftwareDisplayName = string.Empty;

        [ProtoMember(12)]
        public string OutsideSoftwareDisplayName
        {
            get
            {
                return _outsideSoftwareDisplayName;
            }

            set
            {
                if (value == _outsideSoftwareDisplayName)
                    return;
                SetProperty(ref _outsideSoftwareDisplayName, value);

            }
        }


        private string _filterText = string.Empty;

        [ProtoMember(13)]
        public string FilterText
        {
            get
            {
                return _filterText;
            }

            set
            {
                if (value == _filterText)
                    return;
                SetProperty(ref _filterText, value);

            }
        }

        private List<string> _lstFilterText = new List<string>();

        [ProtoMember(14)]
        public List<string> LstFilterText
        {
            get
            {
                return _lstFilterText;
            }

            set
            {
                if (value == _lstFilterText)
                    return;
                SetProperty(ref _lstFilterText, value);

            }
        }

        private bool _isFilterByIncommingMessageText;

        [ProtoMember(15)]
        public bool IsFilterByIncommingMessageText
        {
            get
            {
                return _isFilterByIncommingMessageText;
            }

            set
            {
                if (value == _isFilterByIncommingMessageText)
                    return;
                SetProperty(ref _isFilterByIncommingMessageText, value);

            }
        }


        private string _sourceDisplayName = "Message Source";

        [ProtoMember(16)]
        public string SourceDisplayName
        {
            get
            {
                return _sourceDisplayName;
            }

            set
            {
                if (value == _sourceDisplayName)
                    return;
                SetProperty(ref _sourceDisplayName, value);

            }
        }

        private bool _isFilterByMessageRequestText;

        [ProtoMember(17)]
        public bool IsFilterByMessageRequestText
        {
            get
            {
                return _isFilterByMessageRequestText;
            }

            set
            {
                if (value == _isFilterByMessageRequestText)
                    return;
                SetProperty(ref _isFilterByMessageRequestText, value);

            }
        }

       
        private bool _isReplyToPageMessagesChecked;
        [ProtoMember(18)]
        public bool IsReplyToPageMessagesChecked
        {
            get { return _isReplyToPageMessagesChecked; }
            set
            {
                if (value == _isReplyToPageMessagesChecked)
                    return;
                SetProperty(ref _isReplyToPageMessagesChecked, value);
            }
        }


        private string _ownPages = string.Empty;
        [ProtoMember(19)]
        public string OwnPages
        {
            get
            {
                return _ownPages;
            }

            set
            {
                if (value == _ownPages)
                    return;
                SetProperty(ref _ownPages, value);
            }
        }

        private void ChangeCount(bool value)
        {
            try
            {
                if (value)
                    Count++;
                else
                    Count--;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /*private void ChangeTypeCount(bool value)
        {
            try
            {
                if (value)
                    TypeCount++;
                else
                    TypeCount--;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }*/

    }

}
