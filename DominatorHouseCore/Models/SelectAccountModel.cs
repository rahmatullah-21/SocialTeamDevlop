using System.Collections.ObjectModel;
using System.Windows;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class SelectAccountModel:BindableBase
    {
        private bool _isAccountSelected;
        public bool IsAccountSelected
        {
            get
            {
                return _isAccountSelected;
            }
            set
            {
                if (_isAccountSelected == value)
                    return;
                SetProperty(ref _isAccountSelected, value);
               
            }
        }
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName == value)
                    return;
                SetProperty(ref _userName, value);

            }
        }
        private string _groupName;
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName == value)
                    return;
                SetProperty(ref _groupName, value);

            }
        }
        private ObservableCollection<ContentSelectGroup> _groups = new ObservableCollection<ContentSelectGroup>();
        public ObservableCollection<ContentSelectGroup> Groups
        {
            get
            {
                return _groups;
            }
            set
            {
                if (_groups == value)
                    return;
                SetProperty(ref _groups, value);

            }
        }
        private string _groupText = "LangKeySelectGroups".FromResourceDictionary()?.ToString();
        public string GroupText
        {
            get { return _groupText; }
            set { SetProperty(ref _groupText, value); }
        }
    }
}