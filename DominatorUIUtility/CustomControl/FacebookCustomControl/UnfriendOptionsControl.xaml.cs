using DominatorHouseCore.Models.FacebookModels;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace LegionUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    /// Interaction logic for UnfriendOptionsControl.xaml
    /// </summary>
    public partial class UnfriendOptionsControl
    {
        public UnfriendOptionsControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public UnfriendOption UnfriendOptionModel
        {
            get { return (UnfriendOption)GetValue(UnfriendOptionProperty); }
            set { SetValue(UnfriendOptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnfriendOptionProperty =
            DependencyProperty.Register("UnfriendOptionModel", typeof(UnfriendOption), typeof(UnfriendOptionsControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public bool IsCustomOptionsRequired
        {
            get { return (bool)GetValue(IsCustomOptionsRequiredProperty); }
            set { SetValue(IsCustomOptionsRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCustomOptionsRequiredProperty =
            DependencyProperty.Register("IsCustomOptionsRequired", typeof(bool), typeof(UnfriendOptionsControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public bool IsSourceTypeRequired
        {
            get { return (bool)GetValue(IsSourceTypeRequiredProperty); }
            set { SetValue(IsSourceTypeRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSourceTypeRequiredProperty =
            DependencyProperty.Register("IsSourceTypeRequired", typeof(bool), typeof(UnfriendOptionsControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public bool MessageOptionsVisibility
        {
            get { return (bool)GetValue(MessageOptionsVisibilityProperty); }
            set { SetValue(MessageOptionsVisibilityProperty, value); }
        }

        //Using a DependencyProperty as the backing store for PostFilter.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageOptionsVisibilityProperty =
            DependencyProperty.Register("MessageOptionsVisibility", typeof(bool), typeof(UnfriendOptionsControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }


        private void TypeChecked(object sender, RoutedEventArgs e)
        {
            UnfriendOptionModel.TypeCount++;
        }

        private void TypeUnChecked(object sender, RoutedEventArgs e)
        {
            UnfriendOptionModel.TypeCount--;
        }

        private void Source_UnChecked(object sender, RoutedEventArgs e)
        {
            UnfriendOptionModel.Count--;
        }

        private void Source_Checked(object sender, RoutedEventArgs e)
        {
            UnfriendOptionModel.Count++;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            UnfriendOptionModel.LstFilterText = Regex.Split(UnfriendOptionModel.FilterText, "\r\n").ToList();
        }
    }
}
