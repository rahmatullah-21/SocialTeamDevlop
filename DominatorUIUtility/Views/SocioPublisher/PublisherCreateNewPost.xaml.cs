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
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherCreateNewPost.xaml
    /// </summary>
    public partial class PublisherCreateNewPost : UserControl , INotifyPropertyChanged
    {
        public PublisherCreateNewPost()
        {
            InitializeComponent();
            _publisherPostlistModel = new PublisherPostlistModel();
            _publisherPostlistModel.InitializePostData();
            CreateNewPost.DataContext = PublisherPostlistModel;
        }

        private PublisherPostlistModel _publisherPostlistModel;

        public PublisherPostlistModel PublisherPostlistModel
        {
            get
            {
                return _publisherPostlistModel;
            }
            set
            {            
                _publisherPostlistModel = value;
                OnPropertyChanged(nameof(PublisherPostlistModel));
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
