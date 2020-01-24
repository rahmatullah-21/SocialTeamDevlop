using System.Windows;
using System.Windows.Input;

namespace LegionUIUtility.ViewModel.Startup
{
    public interface INextPreviousViewModel
    {

    }
    public class NextPreviousViewModel : DependencyObject, INextPreviousViewModel
    {
        public ICommand NextCommand
        {
            get { return (ICommand)GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(NextPreviousViewModel));

        public string NextCommandParameter
        {
            get { return (string)GetValue(NextCommandParameterProperty); }
            set { SetValue(NextCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandParameterProperty =
            DependencyProperty.Register("NextCommandParameter", typeof(string), typeof(NextPreviousViewModel), new FrameworkPropertyMetadata(OnAvailableItemsChanged));


        public ICommand PreviousCommand
        {
            get { return (ICommand)GetValue(PreviousCommandProperty); }
            set { SetValue(PreviousCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(NextPreviousViewModel));

        public string PreviousCommandParameter
        {
            get { return (string)GetValue(PreviousCommandParameterProperty); }
            set { SetValue(PreviousCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandParameterProperty =
            DependencyProperty.Register("PreviousCommandParameter", typeof(string), typeof(NextPreviousViewModel), new FrameworkPropertyMetadata(OnAvailableItemsChanged));
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
    }
}
