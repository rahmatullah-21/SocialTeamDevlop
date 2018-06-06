using System.Windows.Input;
using DominatorHouseCore.Command;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDirectPostsViewModel
    {

        #region Constructor

        public PublisherDirectPostsViewModel()
        {
            MultiplePostCommand = new BaseCommand<object>(CanExecuteMultiPost, ExecuteMultiPost);
        }

        #endregion

        #region Properties

        public ICommand MultiplePostCommand { get; set; }

        #endregion

        #region Methods

        public bool CanExecuteMultiPost(object sender) => true;

        public void ExecuteMultiPost(object sender)
        {
                     
        }
        #endregion

    }
}