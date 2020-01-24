using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using CommonServiceLocator;
using DominatorHouseCore.Utility;

namespace LegionUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for EmailNotifications.xaml
    /// </summary>
    public partial class EmailNotifications : UserControl
    {
        private EmailNotificationsModel EmailNotificationsModel { get; set; } = new EmailNotificationsModel();
        IOtherConfigFileManager emailNotifications;
        public EmailNotifications()
        {
            InitializeComponent();
            emailNotifications = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            EmailNotificationsModel = emailNotifications.GetOtherConfig<EmailNotificationsModel>() ?? EmailNotificationsModel;
            MainGrid.DataContext = EmailNotificationsModel;
        }
        private static EmailNotifications ObjEmailNotifications;

        public static EmailNotifications GetSingeltonObjectEmailNotifications()
        {
            return ObjEmailNotifications ?? (ObjEmailNotifications = new EmailNotifications());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (emailNotifications.SaveOtherConfig<EmailNotificationsModel>(EmailNotificationsModel))
                Dialog.ShowDialog("Success", "Email Notifications sucessfully saved !!");
        }
    }
}
