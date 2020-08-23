using DominatorUIUtility.ViewModel.Startup;
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

namespace DominatorUIUtility.Views.AccountSetting
{
    /// <summary>
    /// Interaction logic for JobConfig.xaml
    /// </summary>
    public partial class JobConfig
    {
        public JobConfig(IJobConfigViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
