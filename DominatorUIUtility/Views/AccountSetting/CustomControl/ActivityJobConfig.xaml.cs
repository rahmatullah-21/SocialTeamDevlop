using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    /// Interaction logic for ActivityJobConfig.xaml
    /// </summary>
    public partial class ActivityJobConfig : UserControl
    {
        public ActivityJobConfig()
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
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(ActivityJobConfig), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });


        public string PerJobActivity
        {
            get { return (string)GetValue(PerJobActivityProperty); }
            set { SetValue(PerJobActivityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PerJobActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerJobActivityProperty =
            DependencyProperty.Register("PerJobActivity", typeof(string), typeof(ActivityJobConfig), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultValue = "LangKeyUsers".FromResourceDictionary()
            });

        public string PerHourActivity
        {
            get { return (string)GetValue(PerHourActivityProperty); }
            set { SetValue(PerHourActivityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PerHourActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerHourActivityProperty =
            DependencyProperty.Register("PerHourActivity", typeof(string), typeof(ActivityJobConfig), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultValue = "LangKeyUsers".FromResourceDictionary()
            });

        public string PerDayActivity
        {
            get { return (string)GetValue(PerDayActivityProperty); }
            set { SetValue(PerDayActivityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PerDayActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerDayActivityProperty =
            DependencyProperty.Register("PerDayActivity", typeof(string), typeof(ActivityJobConfig), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultValue = "LangKeyUsers".FromResourceDictionary()
            });

        public string PerWeekActivity
        {
            get { return (string)GetValue(PerWeekActivityProperty); }
            set { SetValue(PerWeekActivityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PerWeekActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerWeekActivityProperty =
            DependencyProperty.Register("PerWeekActivity", typeof(string), typeof(ActivityJobConfig), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultValue = "LangKeyUsers".FromResourceDictionary()
            });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
