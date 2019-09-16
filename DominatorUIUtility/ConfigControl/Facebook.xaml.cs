using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook : UserControl
    {
        private ConfigFacebookModel ConfigFacebookModel { get; set; } = new ConfigFacebookModel();
        private readonly IFBFileManager fbFilemanager;
        private Facebook()
        {
            InitializeComponent();
            fbFilemanager = ServiceLocator.Current.GetInstance<IFBFileManager>();
            ConfigFacebookModel = fbFilemanager.GetFacebookConfig() ?? ConfigFacebookModel;
            MainGrid.DataContext = ConfigFacebookModel;
        }

        private static Facebook ObjFacebook;

        public static Facebook GetSingeltonObjectFacebook()
        {
            return ObjFacebook ?? (ObjFacebook = new Facebook());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (fbFilemanager.SaveFacebookConfig(ConfigFacebookModel))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(), "LangKeyFacebookConfigurationSaved".FromResourceDictionary());
        }
    }
}
