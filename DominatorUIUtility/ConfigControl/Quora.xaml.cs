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
    /// Interaction logic for Quora.xaml
    /// </summary>
    public partial class Quora : UserControl
    {
        private QuoraModel QuoraModel { get; set; } = new QuoraModel();
        private Quora()
        {
            InitializeComponent();
            QuoraModel = GenericFileManager.GetModel<QuoraModel>(ConstantVariable.GetOtherQuoraSettingsFile()) ?? QuoraModel;
            MainGrid.DataContext = QuoraModel;
        }
        private static Quora QuoraInstance;

        public static Quora GetSingeltonObjectQuora()
        {
            return QuoraInstance ?? (QuoraInstance = new Quora());
        }
       
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (GenericFileManager.Overrride(QuoraModel, ConstantVariable.GetOtherQuoraSettingsFile()))
                Dialog.ShowDialog("Success","Quora Configuration sucessfully saved !!");
        }
    }
}
