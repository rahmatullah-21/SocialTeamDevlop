using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
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

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Reblog.xaml
    /// </summary>
    public partial class Reblog : UserControl
    {
        public Reblog(IReblogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
