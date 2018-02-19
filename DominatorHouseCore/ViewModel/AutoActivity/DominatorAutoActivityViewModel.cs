using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.ViewModel.AutoActivity
{
    public class DominatorAutoActivityViewModel : UserControlSwitchViewModel
    {
        private readonly string _name;
        public override string Name => _name;

        public DominatorAutoActivityViewModel(SocialNetworks networks)
        {
            _name = networks.ToString();
        }



    }
}
