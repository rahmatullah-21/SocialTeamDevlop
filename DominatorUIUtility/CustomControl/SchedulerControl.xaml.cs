using System;
using System.Windows.Controls;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for SchedulerControl.xaml
    /// </summary>
    public partial class SchedulerControl : UserControl
    {
        public SchedulerControl()
        {
           // var startTime = DateTime.Now.ToString(@"HH\:mm\:ss");
            var startTime = "00:00:00";
            var endTime = "23:59:59";

            InitializeComponent();
            StartTimePicker.SelectedTime = Convert.ToDateTime(startTime).TimeOfDay;
            EndTimePicker.SelectedTime = Convert.ToDateTime(endTime).TimeOfDay;
        }
    }
}
