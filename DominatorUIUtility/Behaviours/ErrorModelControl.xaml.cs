using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;

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


        private ObservableCollection<AccountDetails> _accounts = new ObservableCollection<AccountDetails>();

        public ObservableCollection<AccountDetails> Accounts
        {
            get
            {
                return _accounts;
            }
            set
            {
                if (_accounts == value)
                    return;
                _accounts = value;
                OnPropertyChanged(nameof(Accounts));
            }
        }


        private bool _isAllAccountSelectedFromList;
        private bool _isAllAccountSelected;

        public bool IsAllAccountSelected
        {
            get
            {
                return _isAllAccountSelected;
            }
            set
            {
                if (_isAllAccountSelected == value)
                    return;
                _isAllAccountSelected = value;
                OnPropertyChanged(nameof(IsAllAccountSelected));
                SelectAllAccount(IsAllAccountSelected);
                _isAllAccountSelectedFromList = false;
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
        private void SelectAllAccount(bool isAllProxySelected)
        {
            if (_isAllAccountSelectedFromList)
                return;

            Accounts.Select(account =>
            {
                account.IsChecked = isAllProxySelected;
                return account;
            }).ToList();

        }
        private void SelectInividual()
        {
            try
            {
                // To check whether all destinations are selected, then make the tick mark on column header

                if (Accounts.All(x => x.IsChecked))
                    IsAllAccountSelected = true;
                else
                {
                    if (IsAllAccountSelected)
                        _isAllAccountSelectedFromList = true;

                    IsAllAccountSelected = false;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
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


        private void AccountChecked(object sender, RoutedEventArgs e)
        {
            SelectInividual();
        }
    }

    public class AccountDetails : INotifyPropertyChanged
    {
        private string _userName;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName == value)
                    return;
                _userName = value;
                OnPropertyChanged(nameof(UserName));
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
                if (_isChecked == value)
                    return;
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
               
            }
        }

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
    }
}
