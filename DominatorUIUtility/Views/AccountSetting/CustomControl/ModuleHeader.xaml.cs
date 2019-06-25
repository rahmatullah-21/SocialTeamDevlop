using DominatorUIUtility.Behaviours;
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

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    /// Interaction logic for ModuleHeader.xaml
    /// </summary>
    public partial class ModuleHeader : UserControl
    {
        public ModuleHeader()
        {
            InitializeComponent();
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

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ModuleHeader), new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public ICommand ExpandCollapseAllCommand
        {
            get { return (ICommand)GetValue(ExpandCollapseAllCommandProperty); }
            set { SetValue(ExpandCollapseAllCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandCollapseAllCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandCollapseAllCommandProperty =
            DependencyProperty.Register("ExpandCollapseAllCommand", typeof(ICommand), typeof(ModuleHeader));


        private void ClpsExpnd_OnClick(object sender, RoutedEventArgs e)
        {
            HeaderHelper.ExpandCollapseAllExpander(sender, IsExpanded);
        }
    }
}
