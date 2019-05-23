using DominatorHouseCore.Enums;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System;
using System.Linq;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Follow.xaml
    /// </summary>
    public partial class Follow : UserControl
    {
        public Follow(IFollowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }
        IFollowViewModel _viewModel;
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.ListQueryType.Clear();
            Enum.GetValues(typeof(QueryType)).Cast<QueryType>().ToList().ForEach(
           query =>
           {
               if (query.IsTwitter())
                   _viewModel.ListQueryType.Add(query.ToString());
           });
        }
    }
}
