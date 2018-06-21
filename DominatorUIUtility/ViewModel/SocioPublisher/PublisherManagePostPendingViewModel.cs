using System;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPendingViewModel : PublisherPostlistBaseViewModel
    {
        public PublisherManagePostPendingViewModel()
        {         
            RemoveInvalidRatioCommand = new BaseCommand<object>(RemoveInvalidRatioCanExecute, RemoveInvalidRatioExecute);
        }

        #region Properties
    
        public ICommand RemoveInvalidRatioCommand { get; set; }
       
        #endregion

        private bool RemoveInvalidRatioCanExecute(object sender) => true;

        private void RemoveInvalidRatioExecute(object sender)
        {

        }

    }
}
