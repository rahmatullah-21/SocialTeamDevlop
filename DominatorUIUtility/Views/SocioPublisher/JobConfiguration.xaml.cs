using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Data;

namespace DominatorUIUtility.Views.SocioPublisher
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
        readonly object _lock;

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
            _lock = new object();
            BindingOperations.EnableCollectionSynchronization(JobConfigurations.LstTimer, _lock);
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
        
        private CancellationTokenSource cancellectionToken { get; set; }
        private void NumericMaxPost_OnValueDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            cancellectionToken = new CancellationTokenSource();
            if (JobConfigurations.MaxPost <= -1)
                return;
            try
            {

                if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1, cancellectionToken);

                if (JobConfigurations.IsRandomizePublishingTimerChecked)
                {
                    GenerateRandomIntervals(JobConfigurations.MaxPost - 1, cancellectionToken);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void NumericMaxPost_OnValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            cancellectionToken = new CancellationTokenSource();
            try
            {
                if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1, cancellectionToken);

                if (JobConfigurations.IsRandomizePublishingTimerChecked)
                {
                    GenerateRandomIntervals(JobConfigurations.MaxPost + 1, cancellectionToken);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        #endregion

        #region Specific Post Interval Generations

        private void SpecificPostGenerateIntervals(int maxCount, CancellationTokenSource cancellectionToken)
        {
           
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
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        JobConfigurations.LstTimer.Clear();
                    });
                    var timeRange = totalSeconds / maxCount;
                    var timeToAddToStartTime = TimeSpan.FromSeconds(timeRange);

                    for (int noOfPost = 0; noOfPost < maxCount; noOfPost++)
                    {
                        cancellectionToken.Token.ThrowIfCancellationRequested();
                        endTime = startTime + timeToAddToStartTime;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            JobConfigurations.LstTimer.Add(new TimeSpanHelper()
                            {
                                StartTime = startTime,
                                MidTime = DateTimeUtilities.GetRandomTime(startTime, endTime, random),
                                EndTime = endTime
                            });
                        });
                        startTime = endTime + TimeSpan.FromSeconds(1);
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        #endregion

        #region Random Post Interval Generation

        private void GenerateRandomIntervals(int maxCount, CancellationTokenSource cancellectionToken)
        {
           
            Random random = new Random();
            var startTime = JobConfigurations.TimeRange.StartTime;
            var endTime = JobConfigurations.TimeRange.EndTime;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        JobConfigurations.LstTimer.Clear();
                    });
                    for (int noOfPost = 0; noOfPost < maxCount; noOfPost++)
                    {
                        cancellectionToken.Token.ThrowIfCancellationRequested();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            JobConfigurations.LstTimer.Add(new TimeSpanHelper() { MidTime = DateTimeUtilities.GetRandomTime(startTime, endTime, random) });
                        });
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        #endregion

        private void OnSelectedTimeChanged(object sender, TimePickerBaseSelectionChangedEventArgs<TimeSpan?> e)
        {
            //if (IsAllowEdit)
            //{
            //    if (JobConfigurations == null)
            //        return;

            //    if (JobConfigurations.IsSpecifyPostingIntervalChecked)
            //        SpecificPostGenerateIntervals(JobConfigurations.MaxPost);

            //    if (JobConfigurations.IsRandomizePublishingTimerChecked)
            //    {
            //        GenerateRandomIntervals(JobConfigurations.MaxPost);
            //    }
            //} 
        }

        private void ChkPostingInterval_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                cancellectionToken = new CancellationTokenSource();
                SpecificPostGenerateIntervals(JobConfigurations.MaxPost, cancellectionToken);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChkRandomizePublishing_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                cancellectionToken = new CancellationTokenSource();
                GenerateRandomIntervals(JobConfigurations.MaxPost, cancellectionToken);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
