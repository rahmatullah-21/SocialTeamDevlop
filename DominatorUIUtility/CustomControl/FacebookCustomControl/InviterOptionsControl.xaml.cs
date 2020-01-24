using System.Windows;
using DominatorHouseCore.Models.FacebookModels;

namespace LegionUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    /// Interaction logic for InviterOptionsControl.xaml
    /// </summary>
    public partial class InviterOptionsControl
    {
        public InviterOptionsControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public InviterOptions InviterOptionsModel
        {
            get { return (InviterOptions)GetValue(InviterOptionsModelProperty); }
            set { SetValue(InviterOptionsModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InviterOptionsModelProperty =
            DependencyProperty.Register("InviterOptionsModel", typeof(InviterOptions), typeof(InviterOptionsControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public bool IsReinviteOptionNeeded
        {
            get { return (bool)GetValue(IsReinviteOptionNeededProperty); }
            set { SetValue(IsReinviteOptionNeededProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReinviteOptionNeededProperty =
            DependencyProperty.Register("IsReinviteOptionNeeded", typeof(bool), typeof(InviterOptionsControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public bool IsInviteInMessageVisible
        {
            get { return (bool)GetValue(IsInviteInMessageVisibleProperty); }
            set { SetValue(IsInviteInMessageVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInviteInMessageVisibleProperty =
            DependencyProperty.Register("IsInviteInMessageVisible", typeof(bool), typeof(InviterOptionsControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });



        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

    }
}
