#region

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

#endregion

namespace DominatorHouseCore.Utility
{
    public class Dialog
    {
        public Window GetCustomDialog(BaseMetroDialog dialog, string title = "")
        {
            var dialogWindow = new MetroWindow
            {
                ShowInTaskbar = true,
                ShowActivated = true,
                Topmost = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.SingleBorderWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowTitleBar = true,
                ShowCloseButton = true,
                WindowTransitionsEnabled = false,
                Background = dialog.Background,
                BorderThickness = new Thickness(0),
                GlowBrush = Brushes.Black,
                Title = title,
                Icon = new BitmapImage(new Uri(ConstantVariable.GetSocinatorIcon()))
            };

            try
            {
                dialogWindow.GlowBrush = dialogWindow.FindResource("AccentColorBrush") as SolidColorBrush;
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }

            dialogWindow.MinHeight = SystemParameters.PrimaryScreenHeight / 4.0;
            dialogWindow.MinWidth = SystemParameters.PrimaryScreenHeight / 1.3;
            dialogWindow.SizeToContent = SizeToContent.WidthAndHeight;
            dialogWindow.Content = dialog;
            return dialogWindow;
        }

        public Window GetMetroWindowWithOutClose(object window, string title)
        {
            var MetroWindow = new MetroWindow
            {
                ShowInTaskbar = true,
                ShowActivated = true,
                Topmost = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.SingleBorderWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowTitleBar = true,
                ShowCloseButton = false,
                WindowTransitionsEnabled = false,
                Title = title,
                BorderThickness = new Thickness(0),
                GlowBrush = Brushes.Black,
                Icon = new BitmapImage(new Uri(ConstantVariable.GetSocinatorIcon()))
            };

            try
            {
                MetroWindow.GlowBrush = MetroWindow.FindResource("AccentColorBrush") as SolidColorBrush;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            MetroWindow.MinHeight = SystemParameters.PrimaryScreenHeight / 4.0 - 80;
            MetroWindow.MinWidth = SystemParameters.PrimaryScreenHeight / 1.3;
            MetroWindow.SizeToContent = SizeToContent.WidthAndHeight;
            MetroWindow.Content = window;
            MetroWindow.MaxHeight = SystemParameters.PrimaryScreenHeight - 100;
            MetroWindow.MaxWidth = SystemParameters.PrimaryScreenWidth - 100;
            return MetroWindow;
        }

        public Window GetMetroWindow(object window, string title)
        {
            var MetroWindow = new MetroWindow
            {
                ShowInTaskbar = true,
                ShowActivated = true,
                Topmost = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.SingleBorderWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowTitleBar = true,
                ShowCloseButton = true,
                WindowTransitionsEnabled = false,
                Title = title,
                BorderThickness = new Thickness(0),
                GlowBrush = Brushes.Black,
                Icon = new BitmapImage(new Uri(ConstantVariable.GetSocinatorIcon())),
                Margin = new Thickness(10)
            };

            try
            {
                MetroWindow.GlowBrush = MetroWindow.FindResource("AccentColorBrush") as SolidColorBrush;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            MetroWindow.MinHeight = SystemParameters.PrimaryScreenHeight / 4.0 - 80;
            MetroWindow.MinWidth = SystemParameters.PrimaryScreenHeight / 1.3;
            MetroWindow.SizeToContent = SizeToContent.WidthAndHeight;
            MetroWindow.Content = window;
            MetroWindow.MaxHeight = SystemParameters.PrimaryScreenHeight - 100;
            MetroWindow.MaxWidth = SystemParameters.PrimaryScreenWidth - 100;
            return MetroWindow;
        }

        public static MetroDialogSettings SetMetroDialogButton(string affirmativeText, string negativeText)
        {
            var metroDialogButton = new MetroDialogSettings
            {
                AffirmativeButtonText = affirmativeText,
                NegativeButtonText = negativeText,
                AnimateShow = true,
                AnimateHide = false,
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };
            return metroDialogButton;
        }


        public static MessageDialogResult ShowDialog(string title, string message)
        {
            return DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, title, message);
        }

        public static MessageDialogResult ShowCustomDialog(string title, string message, string affirmativeText,
            string negativeText)
        {
            return DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, title, message,
                MessageDialogStyle.AffirmativeAndNegative, SetMetroDialogButton(affirmativeText, negativeText));
        }

        public static void CloseDialog(object sender)
        {
            try
            {
                var parentWindow = Window.GetWindow((DependencyObject) sender);
                if (parentWindow != null) parentWindow.Close();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static string GetInputDialog(string title, string message, string defaultText, string firstButtonContent,
            string secondButtonContent)
        {
            try
            {
                var settings = new MetroDialogSettings
                {
                    DefaultText = defaultText,
                    AffirmativeButtonText = firstButtonContent,
                    NegativeButtonText = secondButtonContent
                };
                return DialogCoordinator.Instance.ShowModalInputExternal(Application.Current.MainWindow, title, message,
                    settings);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }
    }
}