using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPublishedViewModel
    {
        public PublisherManagePostPublishedViewModel()
        {
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SelectAllAccountDetailsCommand = new BaseCommand<object>(SelectAccountDetailsCanExecute, SelectAccountDetailsExecute);
            EditCommand = new BaseCommand<object>(EditPostDetailsCanExecute, EditPostDetailsExecute);
            SettingsCommand = new BaseCommand<object>(SettingsCanExecute, SettingsExecute);
        }

        #region Properties
        public ICommand OpenContextMenuCommand { get; set; }
        public ICommand SelectAllAccountDetailsCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
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

        private bool SelectAccountDetailsCanExecute(object sender) => true;

        private void SelectAccountDetailsExecute(object sender)
        {

        }


        private bool EditPostDetailsCanExecute(object sender) => true;

        private void EditPostDetailsExecute(object sender)
        {

        }

        private bool SettingsCanExecute(object sender) => true;

        private void SettingsExecute(object sender)
        {
            var publisherViewUtility = new PublisherViewUtility();
            publisherViewUtility.OpenPostlistSettings();
        }
    }
}
