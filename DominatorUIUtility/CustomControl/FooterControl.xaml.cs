using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for FooterControl.xaml
    /// </summary>
    public partial class FooterControl : UserControl
    {

        public FooterControl()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            list_SelectedAccounts=new List<string>();
        }

        
        public List<string> list_SelectedAccounts
        {
            get
            {
                return (List<string>)GetValue(list_SelectedAccountsProperty);
            }
            set
            {
                SetValue(list_SelectedAccountsProperty, value);
            }
        }
        public static readonly DependencyProperty list_SelectedAccountsProperty =
            DependencyProperty.Register("list_SelectedAccounts", typeof(List<string>), typeof(FooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

      

        public string NoOfAccountsSelected 
        {
            get
            {
                return (string)GetValue(NoOfAccountsSelectedProperty);
            }
            set
            {
                SetValue(NoOfAccountsSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty NoOfAccountsSelectedProperty =
           DependencyProperty.Register("NoOfAccountsSelected", typeof(string), typeof(FooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
           {
               BindsTwoWayByDefault = true
           });


        public Brush AccountsSelectedColor
        {
            get
            {
                return (Brush)GetValue(AccountsSelectedColorProperty);
            }
            set
            {
                SetValue(AccountsSelectedColorProperty, value);
            }
        }

        public static readonly DependencyProperty AccountsSelectedColorProperty =
           DependencyProperty.Register("AccountsSelectedColor", typeof(Brush), typeof(FooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
           {
               BindsTwoWayByDefault = true
           });


        public Visibility IsStatCampaignNowVisible
        {
            get
            {
                return (Visibility)GetValue(IsStatCampaignNowVisibleProperty);
            }
            set
            {
                SetValue(IsStatCampaignNowVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty IsStatCampaignNowVisibleProperty =
           DependencyProperty.Register("IsStatCampaignNowVisible", typeof(Visibility), typeof(FooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
           {
               BindsTwoWayByDefault = true
           });

        public string CampaignManager
        {
            get
            {
                return (string)GetValue(CampaignManagerProperty);
            }
            set
            {
                SetValue(CampaignManagerProperty, value);
            }
        }
        public static readonly DependencyProperty CampaignManagerProperty =
         DependencyProperty.Register("CampaignManager", typeof(string), typeof(FooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
         {
             BindsTwoWayByDefault = true
         });

        /// <summary>
        /// Select accounts event registeration
        /// </summary>

        static readonly RoutedEvent SelectAccountChangedRoutedEvent = EventManager.RegisterRoutedEvent("SelectAccountChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FooterControl));

        public event RoutedEventHandler SelectAccountChanged
        {
            add { AddHandler(SelectAccountChangedRoutedEvent, value); }
            remove { RemoveHandler(SelectAccountChangedRoutedEvent, value); }
        }

        void SelectAccountChangedEventHandler()
        {
            RoutedEventArgs objRoutedEventArgs = new RoutedEventArgs(SelectAccountChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        /// <summary>
        /// Create campaign event registeration
        /// </summary>

        static readonly RoutedEvent CreateCampaignChangedRoutedEvent = EventManager.RegisterRoutedEvent("CreateCampaignChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FooterControl));

        public event RoutedEventHandler CreateCampaignChanged
        {
            add { AddHandler(CreateCampaignChangedRoutedEvent, value); }
            remove { RemoveHandler(CreateCampaignChangedRoutedEvent, value); }
        }

        void CreateCampaignChangedEventHandler()
        {
            RoutedEventArgs objRoutedEventArgs = new RoutedEventArgs(CreateCampaignChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        /// <summary>
        ///  Update campaign event registeration
        /// </summary>
        
        static readonly RoutedEvent UpdateCampaignChangedRoutedEvent = EventManager.RegisterRoutedEvent("UpdateCampaignChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FooterControl));

      

        public event RoutedEventHandler UpdateCampaignChanged
        {
            add { AddHandler(UpdateCampaignChangedRoutedEvent, value); }
            remove { RemoveHandler(UpdateCampaignChangedRoutedEvent, value); }
        }

        void UpdateCampaignChangedEventHandler()
        {
            RoutedEventArgs objRoutedEventArgs = new RoutedEventArgs(UpdateCampaignChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Breakpoint here to see if the new value is being set
            var newValue = e.NewValue;
        }

        private void btnSelectAccount_Click(object sender, RoutedEventArgs e)
        {
            SelectAccountChangedEventHandler();
        }

        private void btnCampaignManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CampaignManager.Equals("Create Campaign", StringComparison.CurrentCultureIgnoreCase))
                {
                    CreateCampaignChangedEventHandler();
                }
                else
                {
                    UpdateCampaignChangedEventHandler();

                }
            }
            catch (Exception ex)
            {

              
            }
        }
    }

}
