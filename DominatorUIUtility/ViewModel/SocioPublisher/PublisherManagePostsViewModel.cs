using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostsViewModel : BindableBase
    {
        public PublisherManagePostsViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            TabChangeCommand = new BaseCommand<object>(TabChangeCanExecute, TabChangeExecute);
            InitializeTabs();
        }

        #region Properties

        public ICommand NavigationCommand { get; set; }

        public ICommand TabChangeCommand { get; set; }

        public List<string> ManagePostTabItems { get; set; } = new List<string>();

        public string SelectedTabs { get; set; } = ConstantVariable.DraftPostList;


        private UserControl _selectedTabsUserControls= PublisherManagePostDrafts.Instance;

        public UserControl SelectedTabsUserControls
        {
            get
            {
                return _selectedTabsUserControls;
            }
            set
            {
                if (Equals(_selectedTabsUserControls, value))
                    return;
                _selectedTabsUserControls = value;
                OnPropertyChanged(nameof(SelectedTabsUserControls));
            }
        }


        #endregion

        private  void InitializeTabs()
        {
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostDraft")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostPending")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostPublished")?.ToString());         
        }





        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherDefaultPage.Instance;
                    break;
            }
        }

        private bool TabChangeCanExecute(object sender) => true;

        private void TabChangeExecute(object sender)
        {
            var selectedButton = sender as string;

            if (selectedButton == ConstantVariable.DraftPostList)
            {
                SelectedTabs = ConstantVariable.DraftPostList;
                SelectedTabsUserControls = PublisherManagePostDrafts.Instance;
            }
            else if (selectedButton == ConstantVariable.PendingPostList)
            {
                SelectedTabs = ConstantVariable.PendingPostList;
                SelectedTabsUserControls = PublisherManagePostPending.Instance;
            }
            else
            {
                SelectedTabs = ConstantVariable.PublishedPostList;
                SelectedTabsUserControls = PublisherManagePostPublished.Instance;
            }
        }

    }
}