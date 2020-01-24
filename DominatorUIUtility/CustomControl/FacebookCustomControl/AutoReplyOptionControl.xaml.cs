using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.FacebookModels;

namespace LegionUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    /// Interaction logic for AutoReplyOptionControl.xaml
    /// </summary>
    public partial class AutoReplyOptionControl : UserControl, INotifyPropertyChanged
    {
        public AutoReplyOptionControl()
        {       
            InitializeComponent();
            //AutoReplyOptionModel = AutoReplyOptionModel == null ? new AutoReplyOptionModel() : AutoReplyOptionModel;
            //AutoReplyOptionControlViewModel.AutoReplyOptionModel = AutoReplyOptionModel;
            MainGrid.DataContext = this;
        }


        //private AutoReplyOptionControlViewModel _autoReplyOptionControlViewModel = new AutoReplyOptionControlViewModel();
        //public AutoReplyOptionControlViewModel AutoReplyOptionControlViewModel
        //{
        //    get
        //    {
        //        return _autoReplyOptionControlViewModel;
        //    }
        //    set
        //    {
        //        _autoReplyOptionControlViewModel = value;
        //        OnPropertyChanged(nameof(AutoReplyOptionControlViewModel));
        //    }
        //}


        public AutoReplyOptionModel AutoReplyOptionModel
        {
            get { return (AutoReplyOptionModel)GetValue(UnfriendOptionProperty); }
            set { SetValue(UnfriendOptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnfriendOptionProperty =
            DependencyProperty.Register("AutoReplyOptionModel", typeof(AutoReplyOptionModel), typeof(AutoReplyOptionControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });




        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        
       
        public ICommand CheckedChangedCommand
        {
            get { return (ICommand)GetValue(CheckedChangedCommandProperty); }
            set { SetValue(CheckedChangedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedChangedCommandProperty =
            DependencyProperty.Register("CheckedChangedCommand", typeof(ICommand), typeof(AutoReplyOptionControl));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(AutoReplyOptionControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

