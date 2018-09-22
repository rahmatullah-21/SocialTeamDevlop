using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace DominatorHouse.Support.Logs.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView
    {

        public IEnumerable<SocialNetworks> AvailableNetworks
        {
            get => (IEnumerable<SocialNetworks>)GetValue(AvailableNetworksProperty);
            set => SetValue(AvailableNetworksProperty, value);
        }

        public ILogViewModel ViewModel
        {
            get => (ILogViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ILogViewModel), typeof(LogView), new PropertyMetadata(null));


        public static readonly DependencyProperty AvailableNetworksProperty =
            DependencyProperty.Register("AvailableNetworks", typeof(IEnumerable<SocialNetworks>), typeof(LogView), new PropertyMetadata(null));


        public LogView()
        {
            InitializeComponent();
        }
    }
}
