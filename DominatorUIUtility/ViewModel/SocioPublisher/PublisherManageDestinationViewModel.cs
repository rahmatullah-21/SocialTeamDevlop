using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.FileManagers;
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
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            InitializeDefaultDestinations();
        }

        public ICommand NavigationCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand DeleteDestinationCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }
        public Visibility HeaderVisibility { get; set; }
        public ObservableCollection<PublisherManageDestinationModel> ListPublisherManageDestinationModels
        {
            get
            {
                return _listPublisherManageDestinationModels;
            }
            set
            {
                if (_listPublisherManageDestinationModels == value)
                    return;
                _listPublisherManageDestinationModels = value;
                SetProperty(ref _listPublisherManageDestinationModels, value);
            }
        }

        private ICollectionView _publisherManageDestinationModelView;

        public ICollectionView PublisherManageDestinationModelView
        {
            get
            {
                return _publisherManageDestinationModelView;
            }
            set
            {
                SetProperty(ref _publisherManageDestinationModelView, value);
            }
        }


        private bool _isAllDestinationSelected;
        private ObservableCollection<PublisherManageDestinationModel> _listPublisherManageDestinationModels = new ObservableCollection<PublisherManageDestinationModel>();

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
                        = PublisherCreateDestination.Instance;
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

        private bool OpenContextMenuCanExecute(object sender) => true;

        private void OpenContextMenuExecute(object sender)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button)sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
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
                ManageDestinationFileManager.Delete(d => d.DestinationId != null);
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

                publisherManageDestinationModel.ForEach(x =>
                ListPublisherManageDestinationModels.Remove(x)
                );
                ManageDestinationFileManager.DeleteSelected(publisherManageDestinationModel);
            }
        }

        private List<PublisherManageDestinationModel> GetSelectedDestinations()
            => ListPublisherManageDestinationModels.Where(x => x.IsSelected).ToList();


        public void InitializeDefaultDestinations()
        {
            PublisherManageDestinationModelView = CollectionViewSource.GetDefaultView(ListPublisherManageDestinationModels);
            var savedDestinations = ManageDestinationFileManager.GetAll();
            savedDestinations.ForEach(x => { AddDestinations(x, false); });
        }

        public bool AddDestinations(PublisherManageDestinationModel publisherManageDestinationModel, bool isNewDestination)
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

                if (isNewDestination)
                    ManageDestinationFileManager.Add(publisherManageDestinationModel);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool UpdateDestinations(PublisherManageDestinationModel publisherManageDestinationModel)
        {                   
            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        var destination = ListPublisherManageDestinationModels.FirstOrDefault(x =>   x.DestinationId == publisherManageDestinationModel.DestinationId);

                        if (destination == null)
                            return;

                        destination.AccountCount = publisherManageDestinationModel.AccountCount;
                        destination.CampaignsCount = publisherManageDestinationModel.CampaignsCount;
                        destination.CreatedDate = publisherManageDestinationModel.CreatedDate;
                        destination.DestinationId = publisherManageDestinationModel.DestinationId;
                        destination.DestinationName = publisherManageDestinationModel.DestinationName;
                        destination.GroupsCount = publisherManageDestinationModel.GroupsCount;
                        destination.IsSelected = publisherManageDestinationModel.IsSelected;
                        destination.PagesOrBoardsCount = publisherManageDestinationModel.PagesOrBoardsCount;
                        destination.WallsOrProfilesCount = publisherManageDestinationModel.WallsOrProfilesCount;
                    });
                else
                {
                    var destination = ListPublisherManageDestinationModels.FirstOrDefault(x =>  x.DestinationId == publisherManageDestinationModel.DestinationId);
                    if (destination != null)
                    {
                        destination.AccountCount = publisherManageDestinationModel.AccountCount;
                        destination.CampaignsCount = publisherManageDestinationModel.CampaignsCount;
                        destination.CreatedDate = publisherManageDestinationModel.CreatedDate;
                        destination.DestinationId = publisherManageDestinationModel.DestinationId;
                        destination.DestinationName = publisherManageDestinationModel.DestinationName;
                        destination.GroupsCount = publisherManageDestinationModel.GroupsCount;
                        destination.IsSelected = publisherManageDestinationModel.IsSelected;
                        destination.PagesOrBoardsCount = publisherManageDestinationModel.PagesOrBoardsCount;
                        destination.WallsOrProfilesCount = publisherManageDestinationModel.WallsOrProfilesCount;
                    }
                }

                ManageDestinationFileManager.UpdateDestinations(ListPublisherManageDestinationModels);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public PublisherManageDestinationModel GetManageDestination(string destinationId)
        {
            return ListPublisherManageDestinationModels.FirstOrDefault(x => x.DestinationId == destinationId);
        }
    }
}
