using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for UserFiltersControl.xaml
    /// </summary>
    public partial class UserFiltersControl : UserControl
    {
        public UserFiltersControl()
        {
            InitializeComponent();
        }


        public UserFilterModel UserFilter
        {
            get { return (UserFilterModel)GetValue(UserFilterProperty); }
            set { SetValue(UserFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserFilterProperty =
            DependencyProperty.Register("UserFilter", typeof(UserFilterModel), typeof(UserFiltersControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }



        private static readonly RoutedEvent SaveUserFilterEvent =
            EventManager.RegisterRoutedEvent("SaveUserFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UserFiltersControl));

        public event RoutedEventHandler SaveUserFilterEventHandler
        {
            add { AddHandler(SaveUserFilterEvent, value); }
            remove { RemoveHandler(SaveUserFilterEvent, value); }
        }


        void SaveUserFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveUserFilterEvent);
            RaiseEvent(rountedargs);

        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveUserFilterEventArgsHandler();
        }

        private void CaptionPostInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.CaptionPosts = CaptionPostInputBox.InputText;
            UserFilter.LstPostCaption = Regex.Split(CaptionPostInputBox.InputText, "\r\n").ToList();

        }

        private void InvalidWordsInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.InvalidWords = InvalidWordsInputBox.InputText;
            UserFilter.LstInvalidWord = Regex.Split(InvalidWordsInputBox.InputText, "\r\n").ToList();
        }
    }
}
