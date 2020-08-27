using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Command;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for SelectOptionControl.xaml
    /// </summary>
    public partial class SelectOptionControl
    {
        public static readonly DependencyProperty SelectedInputOptionProperty =
            DependencyProperty.Register("SelectedInputOption", typeof(string), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty SelectedOptionDisplayNameProperty =
            DependencyProperty.Register("SelectedOptionDisplayName", typeof(string), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstSelectedInputProperty =
            DependencyProperty.Register("LstSelectedInput", typeof(IEnumerable<string>), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsSelectButtonVisibleProperty =
            DependencyProperty.Register("IsSelectButtonVisible", typeof(bool), typeof(SelectOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public SelectOptionControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            DialogParticipation.SetRegister(this, this);
            SaveCommandBinding = new BaseCommand<object>(sender => true, UserInputOnSaveExecute);
            SelectOptionCommandBinding = new BaseCommand<object>(sender => true, SelectOptionCommandExecute);
            LoadTextBoxes();
        }

        //private void LoadPages()
        //{
        //    SelectAccountDetailsControl SelectAccountDetailsControl = null;

        //    var model = SelectOptionModel.SelectPageDetailsModel;

        //    List<FbEntityTypes> hiddenColumnList = new List<FbEntityTypes>();

        //    hiddenColumnList.Add(FbEntityTypes.Friend);
        //    hiddenColumnList.Add(FbEntityTypes.Group);

        //    List<string> listAccountIds = AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.AccountId).ToList();

        //    if (SelectOptionModel.SelectPageDetailsModel.AccountPagesBoardsPair.Count == 0)
        //    {
        //        var selectedAccounts = SelectOptionModel.SelectPageDetailsModel.AccountPagesBoardsPair.Select(y => y.Key).ToList();

        //        listAccountIds = listAccountIds.Where(x => selectedAccounts.All(y => y == x)).ToList();

        //        SelectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages", true);
        //    }
        //    else
        //    {
        //        SelectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, model, true);
        //    }

        //    var objDialog = new Dialog();

        //    var window = objDialog.GetMetroWindow(SelectAccountDetailsControl, "Select Account Details");

        //    SelectAccountDetailsControl.BtnSave.Click += (senders, Events) =>
        //    {
        //        try
        //        {
        //            SelectOptionModel.AccountPagesBoardsPair.Clear();
        //            SelectOptionModel.ListCustomPageUrl.Clear();

        //            model = SelectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

        //            model.ListSelectDestination.ForEach(x =>
        //            {
        //                var accountPagespair = model.AccountPagesBoardsPair.Where(y => y.Key == x.AccountId).ToList();

        //                if (x.IsAccountSelected)
        //                {
        //                    SelectOptionModel.AccountPagesBoardsPair.AddRange(accountPagespair);
        //                    SelectOptionModel.ListCustomPageUrl.AddRange(accountPagespair.Select(z => z.Value).ToList());
        //                    SelectOptionModel.ListCustomPageUrl = SelectOptionModel.ListCustomPageUrl.Distinct().ToList();
        //                    SelectOptionModel.ListCustomPageUrl.ForEach(z =>
        //                    {
        //                        if (!SelectOptionModel.CustomPageUrl.Contains(z))
        //                            SelectOptionModel.CustomPageUrl += z + "\r\n";
        //                    });
        //                }
        //                else if (model.AccountPagesBoardsPair.Any(y => y.Key == x.AccountId))
        //                {
        //                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "", $"Destiation is selected but Account is not selected");
        //                }

        //            });


        //            window.Close();
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    };

        //    window.ShowDialog();

        //    SelectOptionModel.SelectPageDetailsModel = SelectAccountDetailsControl.GetSelectAccountModel();
        //}

        //private void LoadFriends()
        //{
        //    SelectAccountDetailsControl selectAccountDetailsControl = null;

        //    var model = SelectOptionModel.SelectFriendsDetailsModel;

        //    List<FbEntityTypes> hiddenColumnList = new List<FbEntityTypes>();

        //    hiddenColumnList.Add(FbEntityTypes.Page);
        //    hiddenColumnList.Add(FbEntityTypes.Group);

        //    List<string> listAccountIds = AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.AccountId).ToList();

        //    if (SelectOptionModel.AccountFriendsPair.Count == 0)
        //    {
        //        var selectedAccounts = SelectOptionModel.SelectFriendsDetailsModel.AccountFriendsPair.Select(y => y.Key).ToList();

        //        listAccountIds = listAccountIds.Where(x => selectedAccounts.All(y => y == x)).ToList();

        //        selectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages");
        //    }
        //    else
        //    {
        //        selectAccountDetailsControl = new SelectAccountDetailsControl(hiddenColumnList, model);
        //    }

        //    var objDialog = new Dialog();

        //    var window = objDialog.GetMetroWindow(selectAccountDetailsControl, "Select Account Details");

        //    selectAccountDetailsControl.BtnSave.Click += (senders, Events) =>
        //    {
        //        try
        //        {
        //            SelectOptionModel.AccountFriendsPair.Clear();
        //            SelectOptionModel.ListCustomTaggedUser.Clear();

        //            model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

        //            model.ListSelectDestination.ForEach(x =>
        //            {
        //                var accountFriendspair = model.AccountFriendsPair.Where(y => y.Key == x.AccountId).ToList();

        //                if (x.IsAccountSelected)
        //                {
        //                    SelectOptionModel.AccountFriendsPair.AddRange(accountFriendspair);
        //                    SelectOptionModel.ListCustomTaggedUser.AddRange(accountFriendspair.Select(z => z.Value).ToList());
        //                    SelectOptionModel.ListCustomTaggedUser = SelectOptionModel.ListCustomTaggedUser.Distinct().ToList();
        //                    SelectOptionModel.ListCustomTaggedUser.ForEach(z =>
        //                    {
        //                        if (!SelectOptionModel.CustomTaggedUser.Contains(z))
        //                            SelectOptionModel.CustomTaggedUser += z + "\r\n";
        //                    });
        //                }
        //                else if (model.AccountFriendsPair.Any(y => y.Key == x.AccountId))
        //                {
        //                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "", $"Destiation is selected but Account is not selected");
        //                }

        //            });


        //            window.Close();
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    };

        //    window.ShowDialog();

        //    SelectOptionModel.SelectFriendsDetailsModel = selectAccountDetailsControl.GetSelectAccountModel();
        //}

        public ICommand SaveCommandBinding { get; set; }

        public ICommand SelectOptionCommandBinding { get; set; }

        //        private static readonly RoutedEvent SelectInputClickEvent = EventManager.RegisterRoutedEvent("SelectInputClick",
        //            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectOptionControl));

        /// <summary>
        ///     Create a RoutedEventHandler for query clicks
        /// </summary>

        //        public event RoutedEventHandler SelectInputClick
        //        {
        //            add { AddHandler(SelectInputClickEvent, value); }
        //            remove { RemoveHandler(SelectInputClickEvent, value); }
        //        }
        //
        //        void SelectInputClickEventHandler()
        //        {
        //            var routedEventArgs = new RoutedEventArgs(SelectInputClickEvent);
        //            RaiseEvent(routedEventArgs);
        //        }

        public string SelectedInputOption
        {
            get => (string) GetValue(SelectedInputOptionProperty);
            set => SetValue(SelectedInputOptionProperty, value);
        }

        public string InputText
        {
            get => (string) GetValue(InputTextProperty);
            set => SetValue(InputTextProperty, value);
        }

        public string SelectedOptionDisplayName
        {
            get => (string) GetValue(SelectedOptionDisplayNameProperty);
            set => SetValue(SelectedOptionDisplayNameProperty, value);
        }

        public IEnumerable<string> LstSelectedInput
        {
            get => (IEnumerable<string>) GetValue(LstSelectedInputProperty);
            set => SetValue(LstSelectedInputProperty, value);
        }

        public bool IsSelectButtonVisible
        {
            get => (bool) GetValue(IsSelectButtonVisibleProperty);
            set => SetValue(IsSelectButtonVisibleProperty, value);
        }

        /*private SelectOptionModel _selectOptionModel = new SelectOptionModel();

        public SelectOptionModel SelectOptionModel
        {
            get
            {
                return _selectOptionModel;
            }
            set
            {
                if(_selectOptionModel != null && _selectOptionModel==value)
                    return;
                _selectOptionModel = value;
            }
        }*/

        private void SelectOptionCommandExecute(object obj)
        {
            ////if (SelectedInputOption == Application.Current.FindResource("LangKeySelectFriends")?.ToString())
            ////{
            ////    LoadFriends();
            ////}
            ////else if (SelectedInputOption == Application.Current.FindResource("LangKeySelectPages")?.ToString())
            ////{
            ////    LoadPages();
            ////}
        }

        private void LoadTextBoxes()
        {
            if (LstSelectedInput != null)
                foreach (var str in LstSelectedInput)
                    InputText = InputText + str + "\r\n";
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void UserInputOnSaveExecute(object sender)
        {
            if (!string.IsNullOrEmpty(InputText))
                LstSelectedInput = Regex.Split(InputText, "\r\n").Where(x => !string.IsNullOrWhiteSpace(x.Trim()))
                    .Select(y => y.Trim()).Distinct().ToList();
            //LstSelectedInput = Regex.Split(InputText, "\r\n").ToList();
            else
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "There is no data to save.");
        }

        //private void SavePages()
        //{
        //    try
        //    {
        //        var pageUrlList = Regex.Split(SelectOptionModel.CustomPageUrl, "\r\n");
        //        pageUrlList.ForEach(x =>
        //        {
        //            SelectOptionModel.ListCustomPageUrl.Add(x);
        //        });

        //        SelectOptionModel.ListCustomPageUrl = SelectOptionModel.ListCustomPageUrl.Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //private void SaveFriends()
        //{
        //    try
        //    {
        //        var pageUrlList = Regex.Split(SelectOptionModel.CustomPageUrl, "\r\n");
        //        pageUrlList.ForEach(x =>
        //        {
        //            SelectOptionModel.ListCustomPageUrl.Add(x);
        //        });

        //        SelectOptionModel.ListCustomPageUrl = SelectOptionModel.ListCustomPageUrl.Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
    }
}