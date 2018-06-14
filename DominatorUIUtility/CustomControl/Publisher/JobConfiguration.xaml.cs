using System;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
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




        private void ChkPostingInterval_OnChecked(object sender, RoutedEventArgs e)
        {
            JobConfigurations.LstTimer.Clear();
            Random random = new Random();

            var startTime = JobConfigurations.TimeRange.StartTime;
            var endTime = JobConfigurations.TimeRange.EndTime;
            var totalSeconds = (int)((endTime - startTime).TotalSeconds);
            try
            {
                var timeRange = totalSeconds / JobConfigurations.MaxPost;
                var timeToAddToStartTime = TimeSpan.FromSeconds(timeRange);

                for (int noOfPost = 0; noOfPost < JobConfigurations.MaxPost; noOfPost++)
                {

                    endTime = startTime + timeToAddToStartTime;

                    JobConfigurations.LstTimer.Add(new TimeSpanHelper()
                    {
                        StartTime = startTime,
                        MidTime = GetRandomTime(startTime, endTime, random),
                        EndTime = endTime
                    });
                    startTime = endTime + TimeSpan.FromSeconds(1);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChkRandomizePublishing_OnChecked(object sender, RoutedEventArgs e)
        {
            JobConfigurations.LstTimer.Clear();
            Random random = new Random();
            var startTime = JobConfigurations.TimeRange.StartTime;
            var endTime = JobConfigurations.TimeRange.EndTime;
            for (int noOfPost = 0; noOfPost < JobConfigurations.MaxPost; noOfPost++)
            {
                JobConfigurations.LstTimer.Add(new TimeSpanHelper() { MidTime = GetRandomTime(startTime, endTime, random) });
            }
        }

        private TimeSpan GetRandomTime(TimeSpan start, TimeSpan end, Random random)
        {
            try
            {
                int totalSeconds = (int)((end - start).TotalSeconds);
                int nextSeconds = random.Next(totalSeconds);
                return start.Add(TimeSpan.FromSeconds(nextSeconds));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return start;
            }
        }


        private void NumericMaxPost_OnValueDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                JobConfigurations.LstTimer.RemoveAt((int)JobConfigurations.MaxPost - 1);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message + ex.StackTrace);
            }
        }

        private void NumericMaxPost_OnValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            Random random = new Random();
            var startTime = JobConfigurations.TimeRange.StartTime;
            var endTime = JobConfigurations.TimeRange.EndTime;
            if (JobConfigurations.IsRandomizePublishingTimerChecked)
            {
                JobConfigurations.LstTimer.Add(new TimeSpanHelper() { MidTime = GetRandomTime(startTime, endTime, random) });

            }
            else if (JobConfigurations.IsSpecifyPostingIntervalChecked)
            {
                var totalSeconds = (int)((endTime - startTime).TotalSeconds);
                try
                {
                    var timeRange = totalSeconds / JobConfigurations.MaxPost+1;
                    var timeToAddToStartTime = TimeSpan.FromSeconds(timeRange);

                    endTime = startTime + timeToAddToStartTime;

                    JobConfigurations.LstTimer.Add(new TimeSpanHelper()
                    {
                        StartTime = startTime,
                        MidTime = GetRandomTime(startTime, endTime, random),
                        EndTime = endTime
                    });
                    startTime = endTime + TimeSpan.FromSeconds(1);

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

        }
    }
}
