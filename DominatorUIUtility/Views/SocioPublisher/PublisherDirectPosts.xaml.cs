using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherDirectPosts.xaml
    /// </summary>
    public partial class PublisherDirectPosts : UserControl , INotifyPropertyChanged
    {
        public PublisherDirectPosts()
        {
            InitializeComponent();
            DirectPost.DataContext = PublisherDirectPostsViewModel;
        }

        private PublisherDirectPostsViewModel _publisherDirectPostsViewModel=new PublisherDirectPostsViewModel();

        public PublisherDirectPostsViewModel PublisherDirectPostsViewModel
        {
            get
            {
                return _publisherDirectPostsViewModel;
            }
            set
            {
                _publisherDirectPostsViewModel = value;
                OnPropertyChanged(nameof(PublisherDirectPostsViewModel));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
