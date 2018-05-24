using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class PublisherMediaViewerModel : BindableBase
    {
        private ObservableCollection<string> _mediaList = new ObservableCollection<string>();
        /// <summary>
        /// To specify the all media items 
        /// </summary>
        public ObservableCollection<string> MediaList
        {
            get
            {
                return _mediaList;
            }
            set
            {
                if (_mediaList == value)
                    return;
                _mediaList = value;
                OnPropertyChanged(nameof(MediaList));

                if (_mediaList.Count > 0)
                {
                    TotalItem = _mediaList.Count;
                    CurrentItem = 1;
                    MediaUrl = _mediaList[CurrentItem - 1];
                    IsEnablePreviousNext = false;
                    IsEnableGoNext = _mediaList.Count > 1;
                }
            }
        }

        private int _currentItem;
        /// <summary>
        /// To specify the current item index with respective to media lists
        /// </summary>
        public int CurrentItem
        {
            get
            {
                return _currentItem;
            }
            set
            {
                if (_currentItem == value)
                    return;
                _currentItem = value;
                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        private int _totalItem;
        /// <summary>
        /// To specify the total item index with respective to media lists
        /// </summary>
        public int TotalItem
        {
            get
            {
                return _totalItem;
            }
            set
            {
                if (_totalItem == value)
                    return;
                _totalItem = value;
                OnPropertyChanged(nameof(TotalItem));
            }
        }

        private string _mediaUrl = string.Empty;
        /// <summary>
        /// To hold current media item
        /// </summary>
        public string MediaUrl
        {
            get
            {
                return _mediaUrl;
            }
            set
            {
                if (_mediaUrl == value)
                    return;
                _mediaUrl = value;
                OnPropertyChanged(nameof(MediaUrl));
            }
        }

        private bool _isEnableGoNext;
        /// <summary>
        /// To specify whether button enable for go to next
        /// </summary>
        public bool IsEnableGoNext
        {
            get
            {
                return _isEnableGoNext;
            }
            set
            {
                if (_isEnableGoNext == value)
                    return;
                _isEnableGoNext = value;
                OnPropertyChanged(nameof(IsEnableGoNext));
            }
        }

        private bool _isEnablePreviousNext;
        /// <summary>
        /// To specify whether button enable for go to previous
        /// </summary>
        public bool IsEnablePreviousNext
        {
            get
            {
                return _isEnablePreviousNext;
            }
            set
            {
                if (_isEnablePreviousNext == value)
                    return;
                _isEnablePreviousNext = value;
                OnPropertyChanged(nameof(IsEnableGoNext));
            }
        }
    }
}