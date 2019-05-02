using System;
using System.Windows.Input;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace DominatorHouse.ViewModels.Startup
{
    public interface ISelectUserTypeViewModel
    {
        string SelectedUser { get; set; }
        ObservableCollection<string> LstUserType { get; set; }
    }
    public class SelectUserTypeViewModel : BindableBase, ISelectUserTypeViewModel
    {
        public SelectUserTypeViewModel()
        {
            BeginnerCommand = new DelegateCommand<object>(OnBeginnerSelect);
            InitilizeUserType();
        }

        public ICommand BeginnerCommand { get; set; }

        private string _selectedUser = string.Empty;

        public string SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }
        private ObservableCollection<string> _lstUserType = new ObservableCollection<string>();

        public ObservableCollection<string> LstUserType
        {
            get { return _lstUserType; }
            set { SetProperty(ref _lstUserType, value); }
        }

        private void OnBeginnerSelect(object sender)
        {


        }
        void InitilizeUserType()
        {
            var networks = Enum.GetNames(typeof(UsersType));
            networks.ForEach(network =>
            {
                LstUserType.Add(network);
            });
        }
    }


}
