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
using DominatorHouseCore.Enums;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ActivitySpeedControl.xaml
    /// </summary>
    public partial class ActivitySpeedControl : UserControl
    {
        public ActivitySpeedControl()
        {
            InitializeComponent();
            MainPanel.DataContext = this;
        }


        public string AverageCountPerDay
        {
            get { return (string)GetValue(AverageCountPerDayProperty); }
            set { SetValue(AverageCountPerDayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AverageCountPerDay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AverageCountPerDayProperty =
            DependencyProperty.Register("AverageCountPerDay", typeof(string), typeof(ActivitySpeedControl), new PropertyMetadata(0));



        public string AverageCountPerWeek
        {
            get { return (string)GetValue(AverageCountPerWeekProperty); }
            set { SetValue(AverageCountPerWeekProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AverageCountPerWeek.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AverageCountPerWeekProperty =
            DependencyProperty.Register("AverageCountPerWeek", typeof(string), typeof(ActivitySpeedControl), new PropertyMetadata(0));



        public ObservableCollection<ActivitySpeed> ActivitySpeed
        {
            get { return (ObservableCollection<ActivitySpeed>)GetValue(ActivitySpeedProperty); }
            set { SetValue(ActivitySpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActivitySpeedProperty =
            DependencyProperty.Register("ActivitySpeed", typeof(ObservableCollection<ActivitySpeed>), typeof(ActivitySpeedControl), new PropertyMetadata(0));


    }
}
