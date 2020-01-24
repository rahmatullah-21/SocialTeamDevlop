using LegionUIUtility.Behaviours;
using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LegionUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    /// Interaction logic for ModuleHeader.xaml
    /// </summary>
    public partial class ModuleHeader : UserControl
    {
        public ModuleHeader()
        {
            InitializeComponent();
            Header.DataContext = this;
            ExpandCollapseAllCommand = new DelegateCommand(ExpandCollepseAll);
            HeaderHelper.UpdateToggleForNonQuery = UpdateToggleButton;
        }
        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Heading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register("Heading", typeof(string), typeof(ModuleHeader), new PropertyMetadata(string.Empty));
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ModuleHeader), new FrameworkPropertyMetadata(false)
            {
                BindsTwoWayByDefault = true
            });

        public object View
        {
            get { return (object)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for View.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(object), typeof(ModuleHeader));


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        public ICommand ExpandCollapseAllCommand { get; set; }
        bool IsClickedFromToggle { get; set; }
        private void ExpandCollepseAll()
        {
            IsClickedFromToggle = true;
            HeaderHelper.ExpandCollapseAllExpanderForActivity(View, IsExpanded);
        }
        void UpdateToggleButton()
        {
            if (IsClickedFromToggle)
            {
                var isAllCollapsed = HeaderHelper.IsAllExpanderCollapseOrNot(View);
                IsExpanded = !isAllCollapsed;
            }
        }
    }
}
