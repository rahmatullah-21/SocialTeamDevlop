using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
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

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var model = ((FrameworkElement)((FrameworkElement)sender).DataContext).DataContext;

                if (JobConfiguration.SelectedItem == "Slow")
                {
                    var slowSpeed = ((dynamic)model).SlowSpeed;
                    JobConfiguration.ActivitiesPerDay = slowSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = slowSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = slowSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = slowSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = slowSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = slowSpeed.DelayBetweenActivity;
                }
                else if (JobConfiguration.SelectedItem == "Medium")
                {
                    var mediumSpeed = ((dynamic)model).MediumSpeed;
                    JobConfiguration.ActivitiesPerDay = mediumSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = mediumSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = mediumSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = mediumSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = mediumSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = mediumSpeed.DelayBetweenActivity;
                }
                else if (JobConfiguration.SelectedItem == "Fast")
                {
                    var fastSpeed = ((dynamic)model).FastSpeed;
                    JobConfiguration.ActivitiesPerDay = fastSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = fastSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = fastSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = fastSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = fastSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = fastSpeed.DelayBetweenActivity;
                }
                else if (JobConfiguration.SelectedItem == "Superfast")
                {
                    var superfastSpeed = ((dynamic)model).SuperfastSpeed;
                    JobConfiguration.ActivitiesPerDay = superfastSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = superfastSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = superfastSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = superfastSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = superfastSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = superfastSpeed.DelayBetweenActivity;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChkAdvance_OnChecked(object sender, RoutedEventArgs e)
        {
            Speed.SelectedIndex = -1;
        }
    }
}
