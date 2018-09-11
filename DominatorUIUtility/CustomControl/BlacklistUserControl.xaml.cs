using System;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for BlacklistUserControl.xaml
    /// </summary>
    public partial class BlacklistUserControl : UserControl, INotifyPropertyChanged
    {
        public BlacklistUserControl()
        {
            InitializeComponent();
            MainGrid.DataContext = BlackListViewModel;
            BlackListViewModel.InitializeData();
        }

        private BlackListViewModel _blackListViewModel = new BlackListViewModel();

        public BlackListViewModel BlackListViewModel
        {
            get
            {
                return _blackListViewModel;
            }
            set
            {
                if (_blackListViewModel == value)
                    return;
                _blackListViewModel = value;
                OnPropertyChanged(nameof(BlackListViewModel));
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
