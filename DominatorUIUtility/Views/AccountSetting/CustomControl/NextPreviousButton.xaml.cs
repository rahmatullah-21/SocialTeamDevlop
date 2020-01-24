using DominatorHouseCore.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LegionUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    /// Interaction logic for NextPreviousButton.xaml
    /// </summary>
    public partial class NextPreviousButton : UserControl
    {
        public NextPreviousButton()
        {
            InitializeComponent();
            ButtonGrid.DataContext = this;
        }
        public ICommand NextCommand
        {
            get { return (ICommand)GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(NextPreviousButton));

        public string NextCommandParameter
        {
            get { return (string)GetValue(NextCommandParameterProperty); }
            set { SetValue(NextCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandParameterProperty =
            DependencyProperty.Register("NextCommandParameter", typeof(string), typeof(NextPreviousButton), new PropertyMetadata(string.Empty));


        public ICommand PreviousCommand
        {
            get { return (ICommand)GetValue(PreviousCommandProperty); }
            set { SetValue(PreviousCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(NextPreviousButton));

        public string PreviousCommandParameter
        {
            get { return (string)GetValue(PreviousCommandParameterProperty); }
            set { SetValue(PreviousCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandParameterProperty =
            DependencyProperty.Register("PreviousCommandParameter", typeof(string), typeof(NextPreviousButton), new PropertyMetadata(string.Empty));



        public Visibility PreviousVisiblity
        {
            get { return (Visibility)GetValue(PreviousVisiblityProperty); }
            set { SetValue(PreviousVisiblityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousVisiblityProperty =
            DependencyProperty.Register("PreviousVisiblity", typeof(Visibility), typeof(NextPreviousButton), new PropertyMetadata(Visibility.Visible));


        public string NextButtonContent
        {
            get { return (string)GetValue(NextButtonContentProperty); }
            set { SetValue(NextButtonContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextButtonContentProperty =
            DependencyProperty.Register("NextButtonContent", typeof(string), typeof(NextPreviousButton), new PropertyMetadata("LangKeyNext".FromResourceDictionary()));

    }
}
