using DominatorHouseCore.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;

namespace DominatorUIUtility.CustomControl
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    /// Interaction logic for LiveChat.xaml
    /// </summary>
    public partial class LiveChat : UserControl, INotifyPropertyChanged
    {
      
        public LiveChat(SocialNetworks network)
        {

            InitializeComponent();

            LiveChatViewModel = new LiveChatViewModel();
            LiveChatViewModel.SocialNetworks = network;
           
            MainGrid.DataContext = LiveChatViewModel;

            InitilizeDefaultValue(network);
        }

        public void InitilizeDefaultValue(SocialNetworks socialNetworks)
        {
            var accountCustom = AccountCustomControl.GetAccountCustomControl(socialNetworks);

            var accountModel = accountCustom.DominatorAccountViewModel.LstDominatorAccountModel
                .Where(x => x.AccountBaseModel.AccountNetwork == socialNetworks).ToList();

            LiveChatViewModel.LiveChatModel.AccountNames = new ObservableCollection<string>(accountModel.Select(x => x.UserName).ToList());
            
            if (LiveChatViewModel.LiveChatModel.AccountNames.Count > 0)
            {
                LiveChatViewModel.LiveChatModel.DominatorAccountModel = accountModel[0];
                LiveChatViewModel.LiveChatModel.SelectedAccount = LiveChatViewModel.LiveChatModel.AccountNames.First();
                LiveChatViewModel.UpdateFriendList();
            }
            
            try
            {
                //LiveChatViewModel.LstAccountModel =
                //    accountCustom.DominatorAccountViewModel.LstDominatorAccountModel.ToList();

                LiveChatViewModel.LstAccountModel = accountModel;

                LiveChatViewModel.LiveChatModel.DominatorAccountModel =
                    LiveChatViewModel.LstAccountModel.FirstOrDefault(x =>
                        x.UserName == LiveChatViewModel.LstAccountModel[0].UserName.ToString());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private LiveChatViewModel _liveChatViewModel;

        public LiveChatViewModel LiveChatViewModel
        {
            get
            {
                return _liveChatViewModel;
            }
            set
            {
                _liveChatViewModel = value;
                OnPropertyChanged(nameof(LiveChatViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
