using System;
using System.Collections.Generic;
using System.Linq;
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
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for PublisherIndexPage.xaml
    /// </summary>
    public partial class PublisherIndexPage : UserControl
    {
        private PublisherIndexPage()
        {
            InitializeComponent();
            PublisherIndexPageViewModel = new PublisherIndexPageViewModel();
            PublisherIndex.DataContext = PublisherIndexPageViewModel;
        }

        private static PublisherIndexPage _indexPage;

        public static PublisherIndexPage Instance
            = _indexPage ?? (_indexPage = new PublisherIndexPage());

        public PublisherIndexPageViewModel PublisherIndexPageViewModel { get; set; }



    }

    public class PublisherIndexPageViewModel : BindableBase
    {
        private UserControl _selectedUserControl;

        public UserControl SelectedUserControl
        {
            get
            {
                return _selectedUserControl;
            }
            set
            {
                if(_selectedUserControl!=null && Equals(_selectedUserControl, value))
                    return;

                SetProperty(ref _selectedUserControl, value);
            }
        }

        public string LastVisitedPage { get; set; }

    }

}
