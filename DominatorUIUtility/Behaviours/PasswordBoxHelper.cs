using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.Behaviours
{
    public static class PasswordBoxHelper
    {

        public static readonly DependencyProperty PasswordProperty =
              DependencyProperty.RegisterAttached("Password",
              typeof(string), typeof(PasswordBoxHelper),
              new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsPasswordUpdatingProperty =
           DependencyProperty.RegisterAttached("IsPasswordUpdating", typeof(bool),
           typeof(PasswordBoxHelper));
        public static void SetAttach(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(PasswordProperty, value);
        }

        private static bool GetIsPasswordUpdating(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsPasswordUpdatingProperty);
        }

        private static void SetIsPasswordUpdating(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsPasswordUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsPasswordUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsPasswordUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsPasswordUpdating(passwordBox, false);
        }
    
}
}
