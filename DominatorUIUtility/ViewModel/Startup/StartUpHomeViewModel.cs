using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface IStartUpHomeViewModel
    {

    }
    public class StartUpHomeViewModel : BindableBase, IStartUpHomeViewModel
    {
        public StartUpHomeViewModel()
        {

            BeginnerCommand = new DelegateCommand<object>(OnBeginnerSelect);

        }


        public ICommand BeginnerCommand { get; set; }

        private SocialNetworks _selectedNetwork;

        public SocialNetworks SelectedNetwork
        {
            get { return _selectedNetwork; }
            set { SetProperty(ref _selectedNetwork, value); }
        }


        private void OnBeginnerSelect(object sender)
        {


        }

    }

}
