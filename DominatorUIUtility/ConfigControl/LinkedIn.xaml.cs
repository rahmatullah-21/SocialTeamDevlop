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
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for LinkedIn.xaml
    /// </summary>
    public partial class LinkedIn : UserControl
    {
        private LinkedInModel LinkedInModel { get; set; } = new LinkedInModel();
        public LinkedIn()
        {
            InitializeComponent();
            LinkedInModel= GenericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile()) ?? LinkedInModel;
            MainGrid.DataContext = LinkedInModel;
        }

        private static LinkedIn LinkedInInstance;

        public static LinkedIn GetSingeltonObjectLinkedIn()
        {
            return LinkedInInstance ?? (LinkedInInstance = new LinkedIn());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (GenericFileManager.Overrride(LinkedInModel, ConstantVariable.GetOtherLinkedInSettingsFile()))
                Dialog.ShowDialog("Success", "LinkedIn Configuration sucessfully saved !!");
        }
    }
}
