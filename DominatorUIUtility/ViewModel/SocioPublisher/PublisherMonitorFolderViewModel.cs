using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherMonitorFolderViewModel : BindableBase
    {
        private PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;
        public PublisherMonitorFolderViewModel()
        {
            #region Command Initilization

            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            EditCommand = new BaseCommand<object>(EditCanExecute, EditExecute);
            DeleteCommand = new BaseCommand<object>(DeleteCanExecute, DeleteExecute);
            BrowseFolderCommand = new BaseCommand<object>(BrowseFolderCanExecute, BrowseFolderExecute);

            #endregion

        }
        public PublisherMonitorFolderViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            this.tabItemsControl = tabItemsControl;
            LstFolderPath = tabItemsControl.LstFolderPath;
        }

        #region Command

        public ICommand SaveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand BrowseFolderCommand { get; set; }

        #endregion

        #region Properties

        private PublisherMonitorFolderModel _publisherMonitorFolderModel = new PublisherMonitorFolderModel();
        public PublisherMonitorFolderModel PublisherMonitorFolderModel
        {
            get
            {
                return _publisherMonitorFolderModel;
            }
            set
            {
                if (value == _publisherMonitorFolderModel)
                    return;
                SetProperty(ref _publisherMonitorFolderModel, value);
            }
        }

        private ObservableCollection<PublisherMonitorFolderModel> _lstFolderPath = new ObservableCollection<PublisherMonitorFolderModel>();
        public ObservableCollection<PublisherMonitorFolderModel> LstFolderPath
        {
            get
            {
                return _lstFolderPath;
            }
            set
            {
                if (value == _lstFolderPath)
                    return;
                SetProperty(ref _lstFolderPath, value);
            }
        }

        #endregion

        #region Methods

        private bool DeleteCanExecute(object sender) => true;

        private void DeleteExecute(object sender)
        {
            try
            {
                var itemTodelete = ((FrameworkElement)sender).DataContext as PublisherMonitorFolderModel;
                LstFolderPath.Remove(LstFolderPath.FirstOrDefault(x => x.FolderPath == itemTodelete.FolderPath));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool EditCanExecute(object sender) => true;

        private void EditExecute(object sender)
        {
            try
            {
                var itemToEdit = ((FrameworkElement)sender).DataContext as PublisherMonitorFolderModel;
                itemToEdit.ButtonContent = "Update folder path";
                PublisherMonitorFolderModel = itemToEdit;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SaveCanExecute(object sender) => true;
        private void SaveExecute(object sender)
        {

            try
            {
                if (PublisherMonitorFolderModel.ButtonContent == "Save to List")
                {
                    if (!string.IsNullOrEmpty(PublisherMonitorFolderModel.FolderPath) && !LstFolderPath.Any(x => string.Compare(x.FolderPath, PublisherMonitorFolderModel.FolderPath, StringComparison.CurrentCultureIgnoreCase) == 0))
                    {
                        LstFolderPath.Add(new PublisherMonitorFolderModel()
                        {
                            FolderPath = PublisherMonitorFolderModel.FolderPath,
                            FolderTemplate = PublisherMonitorFolderModel.FolderTemplate
                        });

                    }
                }
                else
                {
                    var itemToUpdate = LstFolderPath.FirstOrDefault(x => x.FolderPath == PublisherMonitorFolderModel.FolderPath);
                    itemToUpdate = PublisherMonitorFolderModel;
                }
                PublisherMonitorFolderModel = new PublisherMonitorFolderModel();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool BrowseFolderCanExecute(object arg) => true;

        private void BrowseFolderExecute(object obj)
        {
            var folderPath = FileUtilities.GetExportPath();
            if (!LstFolderPath.Any(x =>
                string.Compare(x.FolderPath, folderPath, StringComparison.CurrentCultureIgnoreCase) == 0))
                PublisherMonitorFolderModel.FolderPath = folderPath;
            else
                Dialog.ShowDialog("Warning", "Folder path already exist.");
        }
        #endregion
    }
}