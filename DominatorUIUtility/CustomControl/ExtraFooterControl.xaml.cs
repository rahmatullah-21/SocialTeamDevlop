using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for ExtraFooterControl.xaml
    /// </summary>
    public partial class ExtraFooterControl
    {
        public static readonly DependencyProperty listSelectedAccountsProperty =
            DependencyProperty.Register("listSelectedAccounts", typeof(List<string>), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty listExtraSelectedProperty =
            DependencyProperty.Register("listExtraSelected", typeof(List<string>), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty NoOfAccountsSelectedProperty =
            DependencyProperty.Register("NoOfAccountsSelected", typeof(string), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty NoOfExtraSelectedProperty =
            DependencyProperty.Register("NoOfExtraSelected", typeof(string), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty AccountsSelectedColorProperty =
            DependencyProperty.Register("AccountsSelectedColor", typeof(Brush), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsStatCampaignNowVisibleProperty =
            DependencyProperty.Register("IsStatCampaignNowVisible", typeof(Visibility), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty CampaignManagerProperty =
            DependencyProperty.Register("CampaignManager", typeof(string), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register("ButtonContent", typeof(string), typeof(ExtraFooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        /// <summary>
        ///     Select accounts event registeration
        /// </summary>
        private static readonly RoutedEvent SelectAccountChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("SelectAccountChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(ExtraFooterControl));

        private static readonly RoutedEvent SelectExtraChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("SelectExtraChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(ExtraFooterControl));

        /// <summary>
        ///     Create campaign event registeration
        /// </summary>
        private static readonly RoutedEvent CreateCampaignChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("CreateCampaignChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ExtraFooterControl));

        /// <summary>
        ///     Update campaign event registeration
        /// </summary>
        private static readonly RoutedEvent UpdateCampaignChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("UpdateCampaignChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(FooterControl));

        public ExtraFooterControl()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            listSelectedAccounts = new List<string>();
        }

        public List<string> listSelectedAccounts
        {
            get => (List<string>) GetValue(listSelectedAccountsProperty);
            set => SetValue(listSelectedAccountsProperty, value);
        }

        public List<string> listExtraSelected
        {
            get => (List<string>) GetValue(listExtraSelectedProperty);
            set => SetValue(listExtraSelectedProperty, value);
        }


        public string NoOfAccountsSelected
        {
            get => (string) GetValue(NoOfAccountsSelectedProperty);
            set => SetValue(NoOfAccountsSelectedProperty, value);
        }

        public string NoOfExtraSelected
        {
            get => (string) GetValue(NoOfExtraSelectedProperty);
            set => SetValue(NoOfExtraSelectedProperty, value);
        }

        public Brush AccountsSelectedColor
        {
            get => (Brush) GetValue(AccountsSelectedColorProperty);
            set => SetValue(AccountsSelectedColorProperty, value);
        }


        public Visibility IsStatCampaignNowVisible
        {
            get => (Visibility) GetValue(IsStatCampaignNowVisibleProperty);
            set => SetValue(IsStatCampaignNowVisibleProperty, value);
        }

        public string CampaignManager
        {
            get => (string) GetValue(CampaignManagerProperty);
            set => SetValue(CampaignManagerProperty, value);
        }

        public string ButtonContent
        {
            get => (string) GetValue(ButtonContentProperty);
            set => SetValue(ButtonContentProperty, value);
        }

        public event RoutedEventHandler SelectAccountChanged
        {
            add => AddHandler(SelectAccountChangedRoutedEvent, value);
            remove => RemoveHandler(SelectAccountChangedRoutedEvent, value);
        }

        private void SelectAccountChangedEventHandler()
        {
            var objRoutedEventArgs = new RoutedEventArgs(SelectAccountChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        public event RoutedEventHandler SelectExtraChanged
        {
            add => AddHandler(SelectExtraChangedRoutedEvent, value);
            remove => RemoveHandler(SelectExtraChangedRoutedEvent, value);
        }

        private void SelectExtraChangedEventHandler()
        {
            var objRoutedEventArgs = new RoutedEventArgs(SelectExtraChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        public event RoutedEventHandler CreateCampaignChanged
        {
            add => AddHandler(CreateCampaignChangedRoutedEvent, value);
            remove => RemoveHandler(CreateCampaignChangedRoutedEvent, value);
        }

        private void CreateCampaignChangedEventHandler()
        {
            var objRoutedEventArgs = new RoutedEventArgs(CreateCampaignChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }


        public event RoutedEventHandler UpdateCampaignChanged
        {
            add => AddHandler(UpdateCampaignChangedRoutedEvent, value);
            remove => RemoveHandler(UpdateCampaignChangedRoutedEvent, value);
        }

        private void UpdateCampaignChangedEventHandler()
        {
            var objRoutedEventArgs = new RoutedEventArgs(UpdateCampaignChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        private void btnSelectAccount_Click(object sender, RoutedEventArgs e)
        {
            SelectAccountChangedEventHandler();
        }

        private void btnCampaignManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CampaignManager.Equals(ConstantVariable.CreateCampaign(),
                    StringComparison.CurrentCultureIgnoreCase))
                    CreateCampaignChangedEventHandler();
                else
                    UpdateCampaignChangedEventHandler();
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