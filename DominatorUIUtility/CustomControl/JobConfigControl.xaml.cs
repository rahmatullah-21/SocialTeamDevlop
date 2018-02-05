using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for JobConfigControl.xaml
    /// </summary>
    public partial class JobConfigControl : UserControl
    {
        public JobConfigControl()
        {
            InitializeComponent();   
            MainGrid.DataContext = this;
        }

        public JobConfiguration JobConfiguration
        {
            get { return (JobConfiguration)GetValue(JobConfigurationProperty); }
            set { SetValue(JobConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for JobConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationProperty =
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(JobConfigControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

    }
}
