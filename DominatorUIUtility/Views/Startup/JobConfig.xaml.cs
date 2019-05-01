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

namespace DominatorUIUtility.Views.Startup
{
    /// <summary>
    /// Interaction logic for JobConfigControl.xaml
    /// </summary>
    public partial class JobConfig : UserControl
    {
        public JobConfig(ISaveSettingViewModel viewModel)
        {
            InitializeComponent();
            JobConfigGrid.DataContext = viewModel;
        }
    }
}
