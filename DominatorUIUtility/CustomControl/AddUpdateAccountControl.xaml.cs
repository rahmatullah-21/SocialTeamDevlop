using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AddUpdateAccountControl.xaml
    /// </summary>
    public partial class AddUpdateAccountControl : UserControl
    {

        public DominatorAccountBaseModel DominatorAccountBaseModel { get; set; } = new DominatorAccountBaseModel();

        /// <summary>
        /// Constructor with default data context
        /// </summary>
        public AddUpdateAccountControl()
        {
            InitializeComponent();

            foreach (var item in Enum.GetValues(typeof(SocialNetworks)))
            {
                ComboBoxSocialNetworks.Items.Add(item);
            }

            UserControlAddUpdateAccount.DataContext = DominatorAccountBaseModel;
        }

        /// <summary>
        /// Constructor with dominatorAccountBaseModel as data context
        /// </summary>
        /// <param name="dominatorAccountBaseModelBinding">Pass the default values which is going to display in view page</param>
        public AddUpdateAccountControl(DominatorAccountBaseModel dominatorAccountBaseModelBinding)
        {
            InitializeComponent();

            btnSave.Content = "Save";
            TextBlockPageTitle.Text = "Add Account";
            CheckBoxShowAdvance.IsChecked = false;
            GridAdvanceOption.Visibility = Visibility.Collapsed;

            DominatorAccountBaseModel = dominatorAccountBaseModelBinding;
            UserControlAddUpdateAccount.DataContext = DominatorAccountBaseModel;
        }

        /// <summary>
        /// Constructor with dominatorAccountBaseModel as data context
        /// </summary>
        /// <param name="dominatorAccountBaseModelBinding">Pass the default values which is going to display in view page</param>
        /// <param name="title">Show the title of the user control, like Add account</param>
        /// <param name="actionButtonContent">Pass the action button content like Save</param>
        /// <param name="showAdvance">Pass true only if proxy ip contains values otherwise false</param>
        public AddUpdateAccountControl(DominatorAccountBaseModel dominatorAccountBaseModelBinding, string title, string actionButtonContent, bool showAdvance,  string socialNetwork)
        {
            InitializeComponent();

            if (socialNetwork==SocialNetworks.Social.ToString())
            {
                foreach (var item in SocinatorInitialize.AvailableNetworks)
                {
                    if(item == SocialNetworks.Social)
                        continue;
                    ComboBoxSocialNetworks.Items.Add(item);
                }
            }
            else
            {
                ComboBoxSocialNetworks.Items.Add((SocialNetworks)Enum.Parse(typeof(SocialNetworks), socialNetwork));
              
                //ComboBoxSocialNetworks.Items.Add(dominatorAccountBaseModelBinding.AccountNetwork);
            }

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
                btnSave.IsDefault=true;
            }
        }
    }
}
