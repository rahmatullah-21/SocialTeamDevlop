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
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for EmailNotifications.xaml
    /// </summary>
    public partial class EmailNotifications : UserControl
    {
        private EmailNotificationsModel EmailNotificationsModel { get; set; }=new EmailNotificationsModel();
        public EmailNotifications()
        {
            InitializeComponent();
            EmailNotificationsModel = EmailNotificationFileManager.GetEmailNotifications() ?? EmailNotificationsModel;
            MainGrid.DataContext = EmailNotificationsModel;
        }
        private static EmailNotifications ObjEmailNotifications;

        public static EmailNotifications GetSingeltonObjectEmailNotifications()
        {
            return ObjEmailNotifications ?? (ObjEmailNotifications = new EmailNotifications());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (EmailNotificationFileManager.SaveEmailNotification(EmailNotificationsModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Email Notifications sucessfully saved !!");
        }
    }
}
