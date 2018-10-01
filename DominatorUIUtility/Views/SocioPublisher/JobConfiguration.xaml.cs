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
        readonly object _lock = new object();

        private static JobConfiguration _jobConfiguration;

        public static JobConfiguration GetInstance(JobConfigurationModel jobConfigurationModel)
        {

            if (_jobConfiguration == null)
                _jobConfiguration = new JobConfiguration(jobConfigurationModel);
            _jobConfiguration.CancelToken();
            _jobConfiguration.LastPostCount = 0;
            _jobConfiguration.JobConfigurations = jobConfigurationModel;
            _jobConfiguration.MainGrid.DataContext = _jobConfiguration.JobConfigurations;

            return _jobConfiguration;
        }


        private JobConfiguration(JobConfigurationModel jobConfigurationModel)
        {
            InitializeComponent();
            JobConfigurations = jobConfigurationModel;
            MainGrid.DataContext = JobConfigurations;

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

            try
            {
                if (LastPostCount != JobConfigurations.MaxPost)
                    CancelToken();

                if (LastPostCount < JobConfigurations.MaxPost - 1)
                {
                    if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                        SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1 - LastPostCount, cancellectionToken, false);

                    if (JobConfigurations.IsRandomizePublishingTimerChecked)
                    {
                        // GenerateRandomIntervals(1, cancellectionToken);
                        RemoveTimeRange(LastPostCount - 1, cancellectionToken);
                    }
                }
                else
                {
                    if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                        SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1, cancellectionToken, false);
                    //SpecificPostGenerateIntervals(LastPostCount - (JobConfigurations.MaxPost - 1), cancellectionToken, false);

                    if (JobConfigurations.IsRandomizePublishingTimerChecked)
                    {
                        // GenerateRandomIntervals(JobConfigurations.MaxPost - 1, cancellectionToken);
                        RemoveTimeRange(LastPostCount - 1, cancellectionToken);
                    }
                }

                LastPostCount = JobConfigurations.MaxPost - 1;
                //if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                //    SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1, cancellectionToken,false);

                //if (JobConfigurations.IsRandomizePublishingTimerChecked)
                //{
                //    GenerateRandomIntervals(JobConfigurations.MaxPost - 1, cancellectionToken);
                //}
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public int LastPostCount { get; set; }
        private void NumericMaxPost_OnValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            if (LastPostCount != JobConfigurations.MaxPost)
                CancelToken();

            try
            {
                if (LastPostCount < JobConfigurations.MaxPost + 1)
                {
                    if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                        SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1 - LastPostCount, cancellectionToken, true);
                    //SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1, cancellectionToken);

                    if (JobConfigurations.IsRandomizePublishingTimerChecked)
                    {
                        GenerateRandomIntervals(JobConfigurations.MaxPost + 1 - LastPostCount, cancellectionToken);
                    }
                    LastPostCount = JobConfigurations.MaxPost + 1;
                }
                else
                {
                    if (JobConfigurations.IsSpecifyPostingIntervalChecked)
                        SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1, cancellectionToken, false);
                    //SpecificPostGenerateIntervals(LastPostCount - (JobConfigurations.MaxPost + 1), cancellectionToken, false);
                    //SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1, cancellectionToken);

                    if (JobConfigurations.IsRandomizePublishingTimerChecked)
                    {
                        GenerateRandomIntervals(LastPostCount - JobConfigurations.MaxPost + 1, cancellectionToken);
                    }
                    LastPostCount = JobConfigurations.MaxPost + 1;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        #endregion

        #region Specific Post Interval Generations

        private void SpecificPostGenerateIntervals(int maxCount, CancellationTokenSource cancellectionToken, bool isNeedToAdd)
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
                    var timeRange = totalSeconds / maxCount;
                    var timeToAddToStartTime = TimeSpan.FromSeconds(timeRange);
                    if (isNeedToAdd)
                        AddTimeRange(maxCount, cancellectionToken, random, startTime, endTime, timeToAddToStartTime);
                    else
                        RemoveTimeRange(maxCount, cancellectionToken);

                }
                catch (Exception ex)
                {
                    if (maxCount == 0)
                        RemoveTimeRange(maxCount, cancellectionToken);
                    ex.DebugLog();
                }
            });
        }

        private void AddTimeRange(int maxCount, CancellationTokenSource cancellectionToken, Random random, TimeSpan startTime, TimeSpan endTime, TimeSpan timeToAddToStartTime)
        {
            for (int noOfPost = 0; noOfPost < maxCount; noOfPost++)
            {
                cancellectionToken.Token.ThrowIfCancellationRequested();
                endTime = startTime + timeToAddToStartTime;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    cancellectionToken.Token.ThrowIfCancellationRequested();
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
        private void RemoveTimeRange(int maxCount, CancellationTokenSource cancellectionToken)
        {
            while (JobConfigurations.LstTimer.Count != maxCount)
            {
                cancellectionToken.Token.ThrowIfCancellationRequested();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    JobConfigurations.LstTimer.RemoveAt(JobConfigurations.LstTimer.Count - 1);
                });

                Thread.Sleep(10);
            }

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

            Task.Factory.StartNew(() =>
            {
                try
                {
                    //   CancelToken();
                    Application.Current.Dispatcher.Invoke(() => JobConfigurations.LstTimer.Clear());

                    //  RemoveTimeRange(JobConfigurations.LstTimer.Count, cancellectionToken);
                    cancellectionToken.Token.ThrowIfCancellationRequested();
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost, cancellectionToken, true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });


        }

        private void ChkRandomizePublishing_OnClick(object sender, RoutedEventArgs e)
        {

            Task.Factory.StartNew(() =>
            {
                try
                {
                    // CancelToken();
                    Application.Current.Dispatcher.Invoke(() => JobConfigurations.LstTimer.Clear());
                    //  RemoveTimeRange(JobConfigurations.LstTimer.Count, cancellectionToken);
                    cancellectionToken.Token.ThrowIfCancellationRequested();
                    GenerateRandomIntervals(JobConfigurations.MaxPost, cancellectionToken);
                    cancellectionToken.Token.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });


        }
        void CancelToken()
        {
            cancellectionToken?.Cancel();
            cancellectionToken?.Dispose();
            cancellectionToken = new CancellationTokenSource();
        }
    }
}
