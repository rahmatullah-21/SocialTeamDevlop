using System;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls;
using DominatorHouseCore.Models.Publisher;

namespace DominatorUIUtility.CustomControl.Publisher
{
    /// <summary>
    /// Interaction logic for JobConfiguration.xaml
    /// </summary>
    public partial class JobConfiguration : UserControl
    {
        public JobConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public JobConfigurationModel JobConfigurations
        {
            get { return (JobConfigurationModel)GetValue(JobConfigurationsProperty); }
            set { SetValue(JobConfigurationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationsProperty =
            DependencyProperty.Register("JobConfigurations", typeof(JobConfigurationModel), typeof(JobConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void numericMaxPost_ValueDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                JobConfigurations.LstTimer.RemoveAt((int)numericMaxPost.Value - 1);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message + ex.StackTrace);
            }

        }

        private void numericMaxPost_ValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                JobConfigurations.LstTimer.Add(new lstTimeSpan() { TimeSpan = new TimeSpan(12, 0, 0) });
            }
            catch (Exception ex)
            {

                GlobusLogHelper.log.Error(ex.Message + ex.StackTrace);
            }
        }
    }
}
