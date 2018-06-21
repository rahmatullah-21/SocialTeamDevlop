using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPendingViewModel : PublisherPostlistBaseViewModel
    {
        public PublisherManagePostPendingViewModel()
        {
            RemoveInvalidRatioCommand = new BaseCommand<object>(RemoveInvalidRatioCanExecute, RemoveInvalidRatioExecute);
        }



        #region Command

        public ICommand RemoveInvalidRatioCommand { get; set; }

        #endregion

        private bool RemoveInvalidRatioCanExecute(object sender) => true;

        private void RemoveInvalidRatioExecute(object sender)
        {

        }


    }
}
