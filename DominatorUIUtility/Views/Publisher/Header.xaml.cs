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

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }


       
         
            public string HeaderText
            {
                get
                {
                    return (string)GetValue(HeaderTextProperty);
                }
                set
                {
                    SetValue(HeaderTextProperty, value);
                }
            }
            public static readonly DependencyProperty HeaderTextProperty =
                DependencyProperty.Register("HeaderText", typeof(string), typeof(Header), new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

            private void BtnBackToCampaign_Click(object sender, RoutedEventArgs e)
            {
                Home ObjPublisher = Home.GetSingletonHome();
                ObjPublisher.managePosts.Visibility = Visibility.Collapsed;
                // ObjPublisher.btnBackToCampaign.Visibility = Visibility.Collapsed;
                ObjPublisher.createCampign.Visibility = Visibility.Collapsed;
                ObjPublisher.manageDestination.Visibility = Visibility.Collapsed;
                ObjPublisher.publisherDetail.Visibility = Visibility.Visible;
                ObjPublisher.publisherPageButtons.Visibility = Visibility.Visible;
            }

        }
    
}
