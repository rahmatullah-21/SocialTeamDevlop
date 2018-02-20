using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

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
                foreach (var item in Enum.GetValues(typeof(SocialNetworks)))
                {
                    ComboBoxSocialNetworks.Items.Add(item);
                }
            }
            else
            {
                ComboBoxSocialNetworks.Items.Add((SocialNetworks)Enum.Parse(typeof(SocialNetworks), socialNetwork));
              
                //ComboBoxSocialNetworks.Items.Add(dominatorAccountBaseModelBinding.AccountNetwork);
            }

            btnSave.Content = !string.IsNullOrEmpty(actionButtonContent) ? actionButtonContent : "Save";
            TextBlockPageTitle.Text = !string.IsNullOrEmpty(title) ? title : "Add Account";
            CheckBoxShowAdvance.IsChecked = showAdvance;
            GridAdvanceOption.Visibility = showAdvance == false ? Visibility.Collapsed : Visibility.Visible;

            DominatorAccountBaseModel = dominatorAccountBaseModelBinding;
            UserControlAddUpdateAccount.DataContext = DominatorAccountBaseModel;

        }


        private void CheckBoxShowAdvance_OnChecked(object sender, RoutedEventArgs e)
        {
            GridAdvanceOption.Visibility = Visibility.Visible;
        }

        private void CheckBoxShowAdvance_OnUnchecked(object sender, RoutedEventArgs e)
        {
            GridAdvanceOption.Visibility = Visibility.Collapsed;
        }


    }
}
