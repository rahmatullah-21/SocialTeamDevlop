using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManageDestinationViewModel : BindableBase
    {

        public PublisherManageDestinationViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            DeleteDestinationCommand = new BaseCommand<object>(DeleteDestinationCanExecute, DeleteDestinationExecute);

            InitializeDefaultDestinations();
        }

        public ICommand NavigationCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand DeleteDestinationCommand { get; set; }

        public ObservableCollection<PublisherManageDestinationModel> ListPublisherManageDestinationModels { get; set; } = new ObservableCollection<PublisherManageDestinationModel>();

        private ICollectionView _publisherManageDestinationModelView;

        public ICollectionView PublisherManageDestinationModelView
        {
            get
            {
                return _publisherManageDestinationModelView;
            }
            set
            {
                if (_publisherManageDestinationModelView != null && _publisherManageDestinationModelView == value)
                    return;
                SetProperty(ref _publisherManageDestinationModelView, value);
            }
        }


        private bool _isAllDestinationSelected;

        public bool IsAllDestinationSelected
        {
            get
            {
                return _isAllDestinationSelected;
            }
            set
            {
                if (_isAllDestinationSelected == value)
                    return;
                SetProperty(ref _isAllDestinationSelected, value);

                if (_isAllDestinationSelected)
                    SelectAllDestination();
                else
                    SelectNoneDestination();
            }
        }

        public void SelectAllDestination()
        {
            ListPublisherManageDestinationModels.Select(x =>
            {
                x.IsSelected = true; return x;
            }).ToList();
        }

        public void SelectNoneDestination()
        {
            ListPublisherManageDestinationModels.Select(x =>
            {
                x.IsSelected = false; return x;
            }).ToList();
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
                case "CreateDestination":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = new PublisherCreateDestination();
                    break;
            }
        }


        private bool SelectionCanExecute(object sender) => true;

        private void SelectionExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "MenuSelectNone":
                case "SelectNone":
                    IsAllDestinationSelected = false;
                    break;

                case "SelectAll":
                case "MenuSelectAll":
                    IsAllDestinationSelected = true;
                    break;
            }
        }


        public bool DeleteDestinationCanExecute(object sender) => true;

        public void DeleteDestinationExecute(object sender)
        {
            var isIndividualDelete = sender is PublisherManageDestinationModel;

            if (isIndividualDelete)
            {
                var destination = (PublisherManageDestinationModel)sender;

                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                    "Confirmation", "If you delete it, cant recover back \nAre you sure ?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                ListPublisherManageDestinationModels.Remove(destination);
            }
            else
            {
                var publisherManageDestinationModel = GetSelectedDestinations();

                if (publisherManageDestinationModel.Count == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                        "Please select atleast one destinations !!");
                    return;
                }

                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                    "Confirmation", "If you delete it will delete all selected destination permanently \nAre you sure ?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                publisherManageDestinationModel.ForEach(x => ListPublisherManageDestinationModels.Remove(x));
            }
        }

        private List<PublisherManageDestinationModel> GetSelectedDestinations()
            => ListPublisherManageDestinationModels.Where(x => x.IsSelected).ToList();


        public void InitializeDefaultDestinations()
        {
            PublisherManageDestinationModelView = CollectionViewSource.GetDefaultView(ListPublisherManageDestinationModels);

            for (int i = 0; i < 5; i++)
            {
                var publisherManageDestinationModel = new PublisherManageDestinationModel();
                publisherManageDestinationModel.GenerateDestinations();
                publisherManageDestinationModel.DestinationName = publisherManageDestinationModel.DestinationName + RandomUtilties.GetRandomNumber(100);
                AddDestinations(publisherManageDestinationModel);
            }
        }

        public bool AddDestinations(PublisherManageDestinationModel publisherManageDestinationModel)
        {
            if (ListPublisherManageDestinationModels.Any(x => x.DestinationName == publisherManageDestinationModel.DestinationName))
            {
                GlobusLogHelper.log.Info("Campaign name already present!");
                return false;
            }

            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        ListPublisherManageDestinationModels.Add(publisherManageDestinationModel);
                    });
                else
                    ListPublisherManageDestinationModels.Add(publisherManageDestinationModel);
            }
            catch (Exception)
            {
                return false;
            }

            return true;

            
        }

        public PublisherCampaignStatusModel GetCampaginDeepClone(PublisherCampaignStatusModel publisherCampaignStatusModel)
            => publisherCampaignStatusModel.DeepClone();

    }
}
