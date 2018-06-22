using System;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPublishedViewModel : PublisherPostlistBaseViewModel
    {
        public PublisherManagePostPublishedViewModel()
        {           
            ExportCommand = new BaseCommand<object>(ExportCanExecute,ExportExecute);
            PublishedDetailsCommand = new BaseCommand<object>(PublishedDetailsCanExecute, PublishedDetailsExecute);
        }

        #region Properties

        public ICommand ExportCommand { get; set; }
        public ICommand PublishedDetailsCommand { get; set; }

        #endregion

        #region Export

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {

        }

        #endregion


        #region  PublishedDetails

        private bool PublishedDetailsCanExecute(object sender) => true;

        private void PublishedDetailsExecute(object sender)
        {
            
            Dialog dialog=new Dialog();
            PublishedPostDetails publishedPostDetails=new PublishedPostDetails();
            var window = dialog.GetMetroWindow(publishedPostDetails, "Published Details");
        } 

        #endregion


    }
}
