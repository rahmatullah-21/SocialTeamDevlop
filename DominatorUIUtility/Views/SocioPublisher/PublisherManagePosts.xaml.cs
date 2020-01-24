using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using LegionUIUtility.ViewModel.SocioPublisher;

namespace LegionUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePosts.xaml
    /// </summary>
    public partial class PublisherManagePosts : UserControl , INotifyPropertyChanged
    {
        public PublisherManagePosts()
        {
            InitializeComponent();
            ManagePost.DataContext = PublisherManagePostsViewModel;
            PublisherManagePostsViewModel.TabChangeExecute(ConstantVariable.DraftPostList);
        }

        private PublisherManagePostsViewModel _publisherManagePostsViewModel = new PublisherManagePostsViewModel();
        public PublisherManagePostsViewModel PublisherManagePostsViewModel
        {
            get
            {
                return _publisherManagePostsViewModel;
            }
            set
            {
                if(_publisherManagePostsViewModel == value)
                    return;
                _publisherManagePostsViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostsViewModel));
            }
        }


        private static PublisherManagePosts _instance;
        public static PublisherManagePosts Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePosts());


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
