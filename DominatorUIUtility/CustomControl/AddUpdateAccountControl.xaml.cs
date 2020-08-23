using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AddUpdateAccountControl.xaml
    /// </summary>
    public partial class AddUpdateAccountControl
    {
        public DominatorAccountBaseModel DominatorAccountBaseModel { get; set; }
        public string JsonCookies { get; set; }
        public string JsonBrowserCookies { get; set; }

        /// <summary>
        /// Constructor with dominatorAccountBaseModel as data context
        /// </summary>
        /// <param name="dominatorAccountBaseModelBinding">Pass the default values which is going to display in view page</param>
        /// <param name="title">Show the title of the user control, like Add account</param>
        /// <param name="actionButtonContent">Pass the action button content like Save</param>
        /// <param name="showAdvance">Pass true only if proxy ip contains values otherwise false</param>
        public AddUpdateAccountControl(DominatorAccountBaseModel dominatorAccountBaseModelBinding, string title, string actionButtonContent, bool showAdvance,
            SocialNetworks socialNetwork)
        {
            InitializeComponent();

            if (socialNetwork == SocialNetworks.Social)
            {
                foreach (var item in SocinatorInitialize.GetRegisterNetwork())
                {
                    if (item == SocialNetworks.Social)
                        continue;
                    ComboBoxSocialNetworks.Items.Add(item);
                }
            }
            else
                ComboBoxSocialNetworks.Items.Add(socialNetwork);

            btnSave.Content = !string.IsNullOrEmpty(actionButtonContent) ? actionButtonContent : "LangKeySave".FromResourceDictionary();
            TextBlockPageTitle.Text = !string.IsNullOrEmpty(title) ? title : "LangKeyAddAccount".FromResourceDictionary();
            CheckBoxShowAdvance.IsChecked = showAdvance;
            GridAdvanceOption.Visibility = showAdvance == false ? Visibility.Collapsed : Visibility.Visible;

            DominatorAccountBaseModel = dominatorAccountBaseModelBinding;
            UserControlAddUpdateAccount.DataContext = DominatorAccountBaseModel;

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSave.IsDefault = true;
            }
        }

        private void SaveCopiedCookies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CopiedJsonCookies.Text?.Trim()))
                    return;

                JsonCookies = CopiedJsonCookies.Text;
                if (DominatorAccountBaseModel.AccountNetwork == SocialNetworks.Facebook)
                    JsonBrowserCookies = CopiedJsonCookies.Text;

                ToasterNotification.ShowSuccess("LangKeyCookiesSavedNowLogin".FromResourceDictionary());
            }
            catch (System.Exception ex)
            {
                if (ex.Message?.Contains(" parsing ") ?? false)
                {
                    ToasterNotification.ShowError("LangKeyCookiesNotInValidJsonText".FromResourceDictionary());
                }
                else
                {
                    ex.DebugLog();
                    ToasterNotification.ShowError("LangKeyOopsAnErrorOccured".FromResourceDictionary());
                }
            }
        }

        private void ClearCopiedCookies_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(JsonCookies))
            {
                ToasterNotification.ShowInfomation("LangKeyNoCookiesAdded".FromResourceDictionary());
                return;
            }
            JsonCookies = JsonBrowserCookies = CopiedJsonCookies.Text = null;
            ToasterNotification.ShowSuccess("LangKeyRemovedCookiesSuccessfully".FromResourceDictionary());
        }
    }
}
