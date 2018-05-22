using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPublishedViewModel : BindableBase
    {
        public PublisherManagePostPublishedViewModel()
        {
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SelectCommand = new BaseCommand<object>(SelectCanExecute, SelectExecute);
            EditCommand = new BaseCommand<object>(EditPostDetailsCanExecute, EditPostDetailsExecute);
            SettingsCommand = new BaseCommand<object>(SettingsCanExecute, SettingsExecute);
            DeleteCommand = new BaseCommand<object>(DeleteCanExecute, DeleteExecute);
            ExportCommand = new BaseCommand<object>(ExportCanExecute,ExportExecute);
        }

        #region Properties

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand SettingsCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand ExportCommand { get; set; }

        private string _campaignId = string.Empty;
        public string CampaignId
        {
            get
            {
                return _campaignId;
            }
            set
            {
                if (_campaignId == value)
                    return;
                _campaignId = value;
                OnPropertyChanged(nameof(CampaignId));
            }
        }
        #endregion

        #region Open Context
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


        #endregion

        private bool SelectCanExecute(object sender) => true;

        private void SelectExecute(object sender)
        {

        }

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {

        }


        private bool EditPostDetailsCanExecute(object sender) => true;

        private void EditPostDetailsExecute(object sender)
        {

        }

        private bool DeleteCanExecute(object sender) => true;

        private void DeleteExecute(object sender)
        {

        }

        private bool SettingsCanExecute(object sender) => true;

        private void SettingsExecute(object sender)
        {
            var publisherViewUtility = new PublisherViewUtility();
            publisherViewUtility.OpenPostlistSettings(CampaignId);
        }

        #region Functionality

        public async Task ReadPostDetails(string campaignId)
        {
            CampaignId = campaignId;
            PublisherViewUtility publisherUtility = new PublisherViewUtility();

        //    var allPosts = await publisherUtility.ReadPostList(campaignId,PostQueuedStatus.Published);

        }

        #endregion

    }
}
