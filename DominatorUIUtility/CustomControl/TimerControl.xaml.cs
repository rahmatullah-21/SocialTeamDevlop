using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Settings;
using DominatorUIUtility.Behaviours;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for TimerControl.xaml
    /// </summary>
    public partial class TimerControl : UserControl
    {
        public TimerControl()
        {
            InitializeComponent();
            RunningTimeItemSource = new RunningTimeSpanModel();
            SundayRunningTimeItemSource=new RunningTimeSpanModel();
            MondayRunningTimeItemSource=new RunningTimeSpanModel();
            TuesdayRunningTimeItemSource=new RunningTimeSpanModel();
            WednesdayRunningTimeItemSource=new RunningTimeSpanModel();
            ThursdayRunningTimeItemSource=new RunningTimeSpanModel();
            FridayRunningTimeItemSource=new RunningTimeSpanModel();
            SaturdayRunningTimeItemSource=new RunningTimeSpanModel();
            MainGrid.DataContext = this;
        }

        public RunningTimeSpanModel RunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(RunningTimeItemSourceProperty); }
            set { SetValue(RunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RunningTimeItemSourceProperty =
            DependencyProperty.Register("RunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public RunningTimeSpanModel SundayRunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(SundayRunningTimeItemSourceProperty); }
            set { SetValue(SundayRunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SundayRunningTimeItemSourceProperty =
            DependencyProperty.Register("SundayRunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public RunningTimeSpanModel MondayRunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(MondayRunningTimeItemSourceProperty); }
            set { SetValue(MondayRunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MondayRunningTimeItemSourceProperty =
            DependencyProperty.Register("MondayRunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public RunningTimeSpanModel TuesdayRunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(TuesdayRunningTimeItemSourceProperty); }
            set { SetValue(TuesdayRunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TuesdayRunningTimeItemSourceProperty =
            DependencyProperty.Register("TuesdayRunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public RunningTimeSpanModel WednesdayRunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(WednesdayRunningTimeItemSourceProperty); }
            set { SetValue(WednesdayRunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WednesdayRunningTimeItemSourceProperty =
            DependencyProperty.Register("WednesdayRunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public RunningTimeSpanModel ThursdayRunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(ThursdayRunningTimeItemSourceProperty); }
            set { SetValue(ThursdayRunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThursdayRunningTimeItemSourceProperty =
            DependencyProperty.Register("ThursdayRunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public RunningTimeSpanModel FridayRunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(FridayRunningTimeItemSourceProperty); }
            set { SetValue(FridayRunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FridayRunningTimeItemSourceProperty =
            DependencyProperty.Register("FridayRunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public RunningTimeSpanModel SaturdayRunningTimeItemSource
        {
            get { return (RunningTimeSpanModel)GetValue(SaturdayRunningTimeItemSourceProperty); }
            set { SetValue(SaturdayRunningTimeItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimeItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaturdayRunningTimeItemSourceProperty =
            DependencyProperty.Register("SaturdayRunningTimeItemSource", typeof(RunningTimeSpanModel), typeof(TimerControl), new FrameworkPropertyMetadata(OnRunningDaysAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
      



      
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            var newValue = e.NewValue;

        }


        public static void OnRunningDaysAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            var newValue = e.NewValue;

            //SundayRunTime 

        }


        private void SundayDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteTiminingRang(sender, SundayRunningTimeItemSource);
          
        }

        private void SundayAddTimingRange_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTimingRange(SundayRunningTimeItemSource);
        }

        private void MondayDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteTiminingRang(sender, MondayRunningTimeItemSource);
           
        }

        private void MondayAddTimingRange_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTimingRange(MondayRunningTimeItemSource);
        }

        private void TuesdayAddTimingRange_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTimingRange(TuesdayRunningTimeItemSource);
        }

        private void TuesdayDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
             DeleteTiminingRang(sender, TuesdayRunningTimeItemSource);
           
        }

        private void WednesdayDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteTiminingRang(sender, WednesdayRunningTimeItemSource);
            
        }

        private void WednesdayAddTimingRange_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTimingRange(WednesdayRunningTimeItemSource);
        }

        private void ThursdayDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteTiminingRang(sender, ThursdayRunningTimeItemSource);
          
        }

        private void ThursdayAddTimingRange_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTimingRange(ThursdayRunningTimeItemSource);
        }

        private void FridayDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteTiminingRang(sender, FridayRunningTimeItemSource);
           
        }

        private void FridayAddTimingRange_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTimingRange(FridayRunningTimeItemSource);
          
        }

        private void SaturdayDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteTiminingRang(sender, SaturdayRunningTimeItemSource);
            

        }

        private void SaturdayAddTimingRange_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTimingRange(SaturdayRunningTimeItemSource);

           
        }

        private void AddTimingRange(RunningTimeSpanModel objRunningTimeSpanModel)
        {
            SchedulerControl objSchedulerControl = new SchedulerControl();

            Dialog objDialog = new Dialog();

            var dialogWindow = objDialog.GetMetroWindow(objSchedulerControl, "Timer");

            objSchedulerControl.btnAddInterval.Click += (senders, Events) =>
            {

                if (objSchedulerControl.StartTimePicker.SelectedTime != null && objSchedulerControl.EndTimePicker.SelectedTime != null)
                {
                    TimeSpan StartTime, EndTime = new TimeSpan();

                    if (objSchedulerControl.StartTimePicker.SelectedTime.Value < objSchedulerControl.EndTimePicker.SelectedTime.Value)
                    {

                        StartTime = objSchedulerControl.StartTimePicker.SelectedTime.Value;
                        EndTime = objSchedulerControl.EndTimePicker.SelectedTime.Value;

                        objRunningTimeSpanModel.Timings.Add(new RunningTimeSpanModel.TimingRange(StartTime, EndTime));

                        dialogWindow.Close();
                    }
                }
                else
                {

                }


            };

            dialogWindow.ShowDialog();
        }

        private void DeleteTiminingRang(object sender, RunningTimeSpanModel objRunningTimeSpanModel)
        {
            try
            {
                var timeToDelete = ((FrameworkElement)sender).DataContext as RunningTimeSpanModel.TimingRange;
             
                objRunningTimeSpanModel.Timings.Remove(objRunningTimeSpanModel.Timings.Where(x => x.StartTime == timeToDelete.StartTime && x.EndTime == timeToDelete.EndTime).Single());
            }
            catch (Exception ex)
            {

               
            }
        }
    }
}
