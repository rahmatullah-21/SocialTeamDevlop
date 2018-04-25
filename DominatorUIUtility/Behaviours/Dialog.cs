using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Media;

namespace DominatorUIUtility.Behaviours
{
   public class Dialog
    {
        public  Window GetCustomDialog(BaseMetroDialog dialog)
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
                GlowBrush = Brushes.Black

            };

            try
            {
                dialogWindow.GlowBrush = dialogWindow.FindResource("AccentColorBrush") as SolidColorBrush;
            }
            catch (Exception)
            {

            }
            dialogWindow.MinHeight = SystemParameters.PrimaryScreenHeight / 4.0;
            dialogWindow.MinWidth = SystemParameters.PrimaryScreenHeight / 1.3;
            dialogWindow.SizeToContent = SizeToContent.WidthAndHeight;
            dialogWindow.Content = dialog;
            return dialogWindow;
        }

        public Window GetMetroWindow(Object window,string title)
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
                Title=title,
                BorderThickness = new Thickness(0),
                GlowBrush = Brushes.Black
            };

            try
            {
                MetroWindow.GlowBrush = MetroWindow.FindResource("AccentColorBrush") as SolidColorBrush;

            }
            catch (Exception)
            {

            }
            MetroWindow.MinHeight = (SystemParameters.PrimaryScreenHeight / 4.0)-80;
            MetroWindow.MinWidth = SystemParameters.PrimaryScreenHeight / 1.3;
            MetroWindow.SizeToContent = SizeToContent.WidthAndHeight;
            MetroWindow.Content = window;
            MetroWindow.MaxHeight = SystemParameters.PrimaryScreenHeight - 100;
            MetroWindow.MaxWidth = SystemParameters.PrimaryScreenWidth - 100;


            return MetroWindow;
        }
        [Obsolete("SetMetroDialogButton without parameter is deprecated, please use SetMetroDialogButton(AffirmativeText,NegativeText) instead.")]
        public static MetroDialogSettings SetMetroDialogButton()
        {
            var metroDialogButton = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                AnimateShow = true,
                AnimateHide = false,
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };
            return metroDialogButton;
        }
        public static MetroDialogSettings SetMetroDialogButton(string affirmativeText, string negativeText)
        {
            var metroDialogButton = new MetroDialogSettings()
            {
                AffirmativeButtonText = affirmativeText,
                NegativeButtonText = negativeText,
                AnimateShow = true,
                AnimateHide = false,
                DefaultButtonFocus=MessageDialogResult.Affirmative
            };
            return metroDialogButton;
        }
    }
}
