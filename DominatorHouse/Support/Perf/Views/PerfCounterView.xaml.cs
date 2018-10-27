using DominatorHouse.ViewModels;
using System.Windows;

namespace DominatorHouse.Support.Perf.Views
{
    /// <summary>
    /// Interaction logic for PerfCounterView.xaml
    /// </summary>
    public partial class PerfCounterView
    {
        public IPerfCounterViewModel ViewModel
        {
            get
            {
                return (IPerfCounterViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(IPerfCounterViewModel), typeof(PerfCounterView), new PropertyMetadata(null));

        public PerfCounterView()
        {
            InitializeComponent();
        }
    }
}
