
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.BusinessLogic.Scheduler;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Process;
using DominatorHouseCore.DatabaseHandler.Utility;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl, INotifyPropertyChanged
    {
        private CampaignViewModel _campaignViewModel = new CampaignViewModel();

        public CampaignViewModel CampaignViewModel
        {
            get
            {
                return _campaignViewModel;
            }
            set
            {
                _campaignViewModel = value;
                OnPropertyChanged(nameof(CampaignViewModel));
            }
        }

        public Campaigns(SocialNetworks socialNetworks)
        {
            InitializeComponent();
            CampaignViewModel.SocialNetworks = socialNetworks;
            CampaignViewModel.SetActivityTypes();
            Campaign.DataContext = CampaignViewModel;
            CampaignViewModel.CampaignCollection = CollectionViewSource.GetDefaultView(CampaignViewModel.LstCampaignDetails);
            instance = this;
        }

        private static Campaigns instance = null;

        public static Campaigns GetCampaignsInstance(SocialNetworks socialNetworks)
        {
            return instance ?? (instance = new Campaigns(socialNetworks));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
