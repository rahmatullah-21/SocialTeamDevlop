using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Collections.Specialized;
using System.Windows;

namespace DominatorHouse.Support.Logs.Views
{
    /// <summary>
    /// Interaction logic for LogList.xaml
    /// </summary>
    public partial class LogList
    {
        public const string LogTypeInfo = "INFO";
        public const string LogTypeError = "ERROR";

        public INotifyCollectionChanged Logs
        {
            get => (INotifyCollectionChanged)GetValue(LogsProperty);
            set => SetValue(LogsProperty, value);
        }

        public ActivityType? ActivityTypeFilter
        {
            get => (ActivityType?)GetValue(ActivityTypeFilterProperty);
            set => SetValue(ActivityTypeFilterProperty, value);
        }

        public SocialNetworks? NetworkFilter
        {
            get => (SocialNetworks?)GetValue(NetworkFilterProperty);
            set => SetValue(NetworkFilterProperty, value);
        }

        public string LogTypeFilter
        {
            get => (string)GetValue(LogTypeFilterProperty);
            set => SetValue(LogTypeFilterProperty, value);
        }


        public LoggerModel SelectedLoggerModel
        {
            get => (LoggerModel)GetValue(SelectedLoggerModelProperty);
            set => SetValue(SelectedLoggerModelProperty, value);
        }

        public static readonly DependencyProperty SelectedLoggerModelProperty =
            DependencyProperty.Register("SelectedLoggerModel", typeof(LoggerModel), typeof(LogList), new PropertyMetadata(null));

        public static readonly DependencyProperty LogTypeFilterProperty =
            DependencyProperty.Register("LogTypeFilter", typeof(string), typeof(LogList), new PropertyMetadata(LogTypeInfo));

        public static readonly DependencyProperty NetworkFilterProperty =
            DependencyProperty.Register("NetworkFilter", typeof(SocialNetworks?), typeof(LogList), new PropertyMetadata(null));

        public static readonly DependencyProperty ActivityTypeFilterProperty =
            DependencyProperty.Register("ActivityTypeFilter", typeof(ActivityType?), typeof(LogList), new PropertyMetadata(null));

        public static readonly DependencyProperty LogsProperty =
            DependencyProperty.Register("Logs", typeof(INotifyCollectionChanged), typeof(LogList), new PropertyMetadata(null));


        public LogList()
        {
            InitializeComponent();
        }
    }
}
