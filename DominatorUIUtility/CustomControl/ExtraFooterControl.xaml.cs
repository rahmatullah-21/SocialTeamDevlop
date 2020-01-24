using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ExtraFooterControl.xaml
    /// </summary>
    public partial class ExtraFooterControl : UserControl
    {
        public ExtraFooterControl()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            listSelectedAccounts = new List<string>();
        }
        public List<string> listSelectedAccounts
        {
            get
            {
                return (List<string>)GetValue(listSelectedAccountsProperty);
            }
            set
            {
                SetValue(listSelectedAccountsProperty, value);
            }
        }
        public static readonly DependencyProperty listSelectedAccountsProperty =
            DependencyProperty.Register("listSelectedAccounts", typeof(List<string>), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public List<string> listExtraSelected
        {
            get
            {
                return (List<string>)GetValue(listExtraSelectedProperty);
            }
            set
            {
                SetValue(listExtraSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty listExtraSelectedProperty =
            DependencyProperty.Register("listExtraSelected", typeof(List<string>), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
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
           DependencyProperty.Register("NoOfAccountsSelected", typeof(string), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
           {
               BindsTwoWayByDefault = true
           });

        public string NoOfExtraSelected
        {
            get
            {
                return (string)GetValue(NoOfExtraSelectedProperty);
            }
            set
            {
                SetValue(NoOfExtraSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty NoOfExtraSelectedProperty =
            DependencyProperty.Register("NoOfExtraSelected", typeof(string), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
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
           DependencyProperty.Register("AccountsSelectedColor", typeof(Brush), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
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
           DependencyProperty.Register("IsStatCampaignNowVisible", typeof(Visibility), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
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
         DependencyProperty.Register("CampaignManager", typeof(string), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
         {
             BindsTwoWayByDefault = true
         });
        public string ButtonContent
        {
            get
            {
                return (string)GetValue(ButtonContentProperty);
            }
            set
            {
                SetValue(ButtonContentProperty, value);
            }
        }
        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register("ButtonContent", typeof(string), typeof(ExtraFooterControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        /// <summary>
        /// Select accounts event registeration
        /// </summary>

        static readonly RoutedEvent SelectAccountChangedRoutedEvent = EventManager.RegisterRoutedEvent("SelectAccountChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtraFooterControl));

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
        static readonly RoutedEvent SelectExtraChangedRoutedEvent = EventManager.RegisterRoutedEvent("SelectExtraChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtraFooterControl));

        public event RoutedEventHandler SelectExtraChanged
        {
            add { AddHandler(SelectExtraChangedRoutedEvent, value); }
            remove { RemoveHandler(SelectExtraChangedRoutedEvent, value); }
        }

        void SelectExtraChangedEventHandler()
        {
            RoutedEventArgs objRoutedEventArgs = new RoutedEventArgs(SelectExtraChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }
        /// <summary>
        /// Create campaign event registeration
        /// </summary>

        static readonly RoutedEvent CreateCampaignChangedRoutedEvent = EventManager.RegisterRoutedEvent("CreateCampaignChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtraFooterControl));

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
                if (CampaignManager.Equals(ConstantVariable.CreateCampaign(), StringComparison.CurrentCultureIgnoreCase))
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
                ex.DebugLog(ex.StackTrace);

            }
        }

        private void BtnSelectExtra_OnClick(object sender, RoutedEventArgs e)
        {
            SelectExtraChangedEventHandler();
        }
    }
}
