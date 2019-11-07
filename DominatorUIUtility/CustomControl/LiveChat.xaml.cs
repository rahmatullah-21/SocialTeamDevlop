using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

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
