using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateDestinationsViewModel
    {
        public PublisherCreateDestinationsViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            InitializeDestinationList();
        }

        public ICommand NavigationCommand { get; set; }

        private bool NavigationCanExecute(object sender) => true;

        public List<PublisherDestinationDetails> DestinationList = new List<PublisherDestinationDetails>();

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherManageDestinations.Instance;
                    break;
            }
        }

        public void InitializeDestinationList()
        {
            DestinationList = PublishDestinationFileManager.GetAll();


        }
    }
}
