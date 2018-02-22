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
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AutoActivityModuleDetails.xaml
    /// </summary>
    public partial class AutoActivityModuleDetails : UserControl
    {
        public AutoActivityModuleDetails()
        {
            InitializeComponent();
            ModuleDetails.DataContext = this;
        }

        public ActivityDetailsModel ActivityDetailsModel
        {
            get { return (ActivityDetailsModel)GetValue(ActivityDetailsModelProperty); }
            set { SetValue(ActivityDetailsModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActivityDetailsModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActivityDetailsModelProperty =
            DependencyProperty.Register("ActivityDetailsModel", typeof(ActivityDetailsModel), typeof(AutoActivityModuleDetails), new PropertyMetadata(new ActivityDetailsModel()));


        /// <summary>
        /// Create a routed event which is registered to event manager with the characteristics 
        /// </summary>
        private static readonly RoutedEvent IsActivityChangedEvent = EventManager.RegisterRoutedEvent("IsActivityChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AutoActivityModuleDetails));

        /// <summary>
        /// Create a RoutedEventHandler for query clicks
        /// </summary>
        public event RoutedEventHandler IsActivityChanged
        {
            add { AddHandler(IsActivityChangedEvent, value); }
            remove { RemoveHandler(IsActivityChangedEvent, value); }
        }

        void IsActivityChangedEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(IsActivityChangedEvent);
            RaiseEvent(routedEventArgs);
        }

        private void ActivityChanged_OnIsCheckedChanged(object sender, EventArgs e)
        {
            IsActivityChangedEventHandler();
        }
    }
}
