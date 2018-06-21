using System;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPublishedViewModel : PublisherPostlistBaseViewModel
    {
        public PublisherManagePostPublishedViewModel()
        {           
            ExportCommand = new BaseCommand<object>(ExportCanExecute,ExportExecute);
        }

        #region Properties

        public ICommand ExportCommand { get; set; }

        #endregion

        #region Export

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {

        }

        #endregion

    }
}
