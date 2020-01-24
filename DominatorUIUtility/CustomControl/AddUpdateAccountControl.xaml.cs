using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Diagnostics;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AddUpdateAccountControl.xaml
    /// </summary>
    public partial class AddUpdateAccountControl : UserControl
    {
        public DominatorAccountBaseModel DominatorAccountBaseModel { get; set; }


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

            //if (socialNetwork == SocialNetworks.Social)
            //{
            //    foreach (var item in SocinatorInitialize.GetRegisterNetwork())
            //    {
            //        if (item == SocialNetworks.Social)
            //            continue;
            //        ComboBoxSocialNetworks.Items.Add(item);
            //    }
            //}
            //else
            //    ComboBoxSocialNetworks.Items.Add(socialNetwork);

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
    }
}
