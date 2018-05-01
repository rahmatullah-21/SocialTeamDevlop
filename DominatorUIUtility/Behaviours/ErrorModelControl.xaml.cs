using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Behaviours
{
    /// <summary>
    /// Interaction logic for ErrorModelControl.xaml
    /// </summary>
    public partial class ErrorModelControl : UserControl, INotifyPropertyChanged
    {
        public ErrorModelControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        #region Properties


        /// <summary>
        /// Implement the INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnPropertyChanged is used to notify that some property are changed 
        /// </summary>
        /// <param name="propertyName">property name</param>        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));




        private ObservableCollection<ErrorModelControl> _accounts = new ObservableCollection<ErrorModelControl>();

        public ObservableCollection<ErrorModelControl> Accounts
        {
            get
            {
                return _accounts;
            }
            set
            {
                _accounts = value; OnPropertyChanged(nameof(Accounts));
            }
        }


        private string _userName;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value; OnPropertyChanged(nameof(UserName));
            }
        }



        private bool _isChecked;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value; OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string WarningText
        {
            get
            {
                return (string)GetValue(WarningTextProperty);
            }
            set
            {
                SetValue(WarningTextProperty, value);
            }
        }

        public static readonly DependencyProperty WarningTextProperty =
            DependencyProperty.Register("WarningText", typeof(string), typeof(ErrorModelControl), new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true
            });

        #endregion

        #region Save button

        private static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent("SaveEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ErrorModelControl));

        public event RoutedEventHandler SaveEventHandler
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }

        void SaveEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveEvent);
            RaiseEvent(rountedargs);
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveEventArgsHandler();
        }

        #endregion

        #region Cancel button

        private static readonly RoutedEvent CancelEvent =
            EventManager.RegisterRoutedEvent("CancelEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ErrorModelControl));

        public event RoutedEventHandler CancelEventHandler
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }


        void CancelEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(CancelEvent);
            RaiseEvent(rountedargs);
        }


        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            CancelEventArgsHandler();
        }


        #endregion


    }
}
