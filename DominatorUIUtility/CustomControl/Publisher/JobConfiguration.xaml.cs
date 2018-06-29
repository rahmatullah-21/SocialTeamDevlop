using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl.Publisher
{
    /// <summary>
    /// Interaction logic for JobConfiguration.xaml
    /// </summary>
    public partial class JobConfiguration : UserControl
    {
        private JobConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            JobConfigurations = new JobConfigurationModel();
        }


        private static JobConfiguration _jobConfiguration;

        public static JobConfiguration GetInstance(JobConfigurationModel jobConfigurationModel)
        {
            if (_jobConfiguration == null)
                _jobConfiguration = new JobConfiguration(jobConfigurationModel);
          
            _jobConfiguration.JobConfigurations = jobConfigurationModel;
            _jobConfiguration.MainGrid.DataContext = _jobConfiguration.JobConfigurations;

            return _jobConfiguration;
        }


        private JobConfiguration(JobConfigurationModel jobConfigurationModel)
        {
            InitializeComponent();
            JobConfigurations = jobConfigurationModel;
            MainGrid.DataContext = JobConfigurations;
        }
        
        #region Properties


        public JobConfigurationModel JobConfigurations { get; set; }

        //public JobConfigurationModel JobConfigurations
        //{
        //    get { return (JobConfigurationModel)GetValue(JobConfigurationsProperty); }
        //    set { SetValue(JobConfigurationsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for RunningTimes.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty JobConfigurationsProperty =
        //    DependencyProperty.Register("JobConfigurations", typeof(JobConfigurationModel), typeof(JobConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
        //    {
        //        BindsTwoWayByDefault = true
        //    });

        //public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var newValue = e.NewValue;
        //}
        #endregion

        #region Post max count changed


        private void NumericMaxPost_OnValueDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            if (JobConfigurations.MaxPost <= -1)
                return;

            if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1);

            if (JobConfigurations.IsRandomizePublishingTimerChecked)
                GenerateRandomIntervals(JobConfigurations.MaxPost - 1);
        }

        private void NumericMaxPost_OnValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1);

            if (JobConfigurations.IsRandomizePublishingTimerChecked)
                GenerateRandomIntervals(JobConfigurations.MaxPost + 1);
        }

        private void NumericMaxPost_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                SpecificPostGenerateIntervals(JobConfigurations.MaxPost);

            if (JobConfigurations.IsRandomizePublishingTimerChecked)
                GenerateRandomIntervals(JobConfigurations.MaxPost);
        }

        #endregion

        #region Specific Post Interval Generations

        private void ChkPostingInterval_OnChecked(object sender, RoutedEventArgs e)
        {
            SpecificPostGenerateIntervals(JobConfigurations.MaxPost);
        }

        private void SpecificPostGenerateIntervals(int maxCount)
        {
            JobConfigurations.LstTimer.Clear();
            var random = new Random();

            var startTime = JobConfigurations.TimeRange.StartTime;
            var endTime = JobConfigurations.TimeRange.EndTime;

            if (startTime > endTime)
            {
                JobConfigurations.LstTimer = new ObservableCollection<TimeSpanHelper>();
                GlobusLogHelper.log.Info("Start time should be greater than end time");
                return;
            }

            var totalSeconds = (int)((endTime - startTime).TotalSeconds);
            try
            {
                var timeRange = totalSeconds / maxCount;
                var timeToAddToStartTime = TimeSpan.FromSeconds(timeRange);

                for (int noOfPost = 0; noOfPost < maxCount; noOfPost++)
                {
                    endTime = startTime + timeToAddToStartTime;

                    JobConfigurations.LstTimer.Add(new TimeSpanHelper()
                    {
                        StartTime = startTime,
                        MidTime = DateTimeUtilities.GetRandomTime(startTime, endTime, random),
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

        #endregion

        #region Random Post Interval Generation

        private void ChkRandomizePublishing_OnChecked(object sender, RoutedEventArgs e)
        {
            GenerateRandomIntervals(JobConfigurations.MaxPost);
        }

        private void GenerateRandomIntervals(int maxCount)
        {
            JobConfigurations.LstTimer.Clear();
            Random random = new Random();
            var startTime = JobConfigurations.TimeRange.StartTime;
            var endTime = JobConfigurations.TimeRange.EndTime;
            for (int noOfPost = 0; noOfPost < maxCount; noOfPost++)
            {
                JobConfigurations.LstTimer.Add(new TimeSpanHelper() { MidTime = DateTimeUtilities.GetRandomTime(startTime, endTime, random) });
            }
        }

        #endregion

        private void OnSelectedTimeChanged(object sender, TimePickerBaseSelectionChangedEventArgs<TimeSpan?> e)
        {
            if (JobConfigurations == null)
                return;

            if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                SpecificPostGenerateIntervals(JobConfigurations.MaxPost);

            if (JobConfigurations.IsRandomizePublishingTimerChecked)
                GenerateRandomIntervals(JobConfigurations.MaxPost);
        }
    }
}
