using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDefaultViewModel
    {
        public PublisherDefaultViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
        }

        public ICommand NavigationCommand { get; set; }

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var moduleName = sender.ToString();

            switch (moduleName)
            {
                case "ManageDestinations":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = PublisherManageDestinations.Instance;
                    break;
                case "ManagePosts":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = new PublisherManagePosts();
                    break;
                case "CreateCampaigns":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = new PublisherCreateCampaigns();
                    break;
            }
        }

       
    }
    }
