using System.Windows;

namespace DominatorHouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            GramDominatorUI.MainWindow GDWindows = new GramDominatorUI.MainWindow();
            GDWindows.Show();
        }

        private void ButtonTD_OnClick(object sender, RoutedEventArgs e)
        {
            TwtDominatorUI.MainWindow TdWindows = new TwtDominatorUI.MainWindow();
            TdWindows.Show();
        }
    }
}
