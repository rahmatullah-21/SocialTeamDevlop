using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;
using System;
using System.Windows.Controls.Primitives;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostPending.xaml
    /// </summary>
    public partial class PublisherManagePostPending : UserControl, INotifyPropertyChanged
    {
        private PublisherManagePostPending()
        {
            InitializeComponent();
            PendingPostLists.DataContext = PublisherManagePostPendingViewModel;
            PostListview.ItemContainerGenerator.StatusChanged += StatusChanged;

        }

        void StatusChanged(object sender, EventArgs e)
        {
            try
            {
                if (PostListview.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    var info = PostListview.Items[PostListview.Items.Count - 1] as PublisherPostlistModel;
                    if (info == null)
                        return;

                    PostListview.ScrollIntoView(info);
                }
            }
            catch (Exception ex)
            {

            }
        }


        private static PublisherManagePostPending _publisherManagePostPending;

        public static PublisherManagePostPending GetPublisherManagePostPending()
            => _publisherManagePostPending ?? (_publisherManagePostPending = new PublisherManagePostPending());

        private PublisherManagePostPendingViewModel _publisherManagePostPendingViewModel = new PublisherManagePostPendingViewModel();
        public PublisherManagePostPendingViewModel PublisherManagePostPendingViewModel
        {
            get
            {
                return _publisherManagePostPendingViewModel;
            }
            set
            {
                if (_publisherManagePostPendingViewModel == value)
                    return;
                _publisherManagePostPendingViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostPendingViewModel));
            }
        }

        //private static PublisherManagePostPending _instance;


        //public static PublisherManagePostPending Instance { get; set; }
        //    = _instance ?? (_instance = new PublisherManagePostPending());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
