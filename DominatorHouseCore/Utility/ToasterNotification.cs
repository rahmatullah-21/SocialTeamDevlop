using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace DominatorHouseCore.Utility
{
    public class ToasterNotification
    {
        public static Notifier Notifier { get; set; }

        public ToasterNotification()
        {
            Notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public static void ShowInfomation(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifier.ShowInformation(message);
                });
            }
            else
            {
                Notifier.ShowInformation(message);
            }
        }

        public static void ShowSuccess(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifier.ShowSuccess(message);
                });
            }
            else
            {
                Notifier.ShowSuccess(message);
            }
        }

        public static void ShowError(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifier.ShowError(message);
                });
            }
            else
            {
                Notifier.ShowError(message);
            }
        }

        public static void ShowWarning(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifier.ShowWarning(message);
                });
            }
            else
            {
                Notifier.ShowWarning(message);
            }
        }

        public static void Dispose()
        {
            Notifier.Dispose();
        }
    }

   
}