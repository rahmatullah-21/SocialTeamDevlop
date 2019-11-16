using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ChatMediaContainer.xaml
    /// </summary>
    public partial class ChatMediaContainer : UserControl
    {
        public ChatMediaContainer()
        {
            InitializeComponent();
        }

        public ObservableCollection<string> ListMediaUrls
        {
            get
            {
                return (ObservableCollection<string>)GetValue(ListMediaUrlsProperty);
            }
            set
            {
                SetValue(ListMediaUrlsProperty, value);
            }
        }


        public static readonly DependencyProperty ListMediaUrlsProperty =
            DependencyProperty.Register("ListMediaUrls", typeof(ObservableCollection<string>), typeof(ChatMediaContainer), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Breakpoint here to see if the new value is being set
            var newValue = e.NewValue;
        }
    }
}
