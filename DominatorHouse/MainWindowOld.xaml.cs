using System.Windows;

namespace DominatorHouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowOld : Window
    {
        public MainWindowOld()
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
#warning UNCOMMENT LINES BELLOW WHEN COMPILED
            //TwtDominatorUI.MainWindow TdWindows = new TwtDominatorUI.MainWindow();
            //TdWindows.Show();
        }
    }
}
