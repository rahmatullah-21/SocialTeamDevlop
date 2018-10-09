using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel.Common;
using System.Collections.Generic;

namespace DominatorHouseCore.ViewModel
{
    public interface ISelectedNetworkViewModel : Common.ISelectableViewModel<SocialNetworks?>
    {
    }

    public class SelectedNetworkViewModel : SelectableViewModel<SocialNetworks?>, ISelectedNetworkViewModel
    {
        public SelectedNetworkViewModel() : base(new List<SocialNetworks?>())
        {
        }
    }
}
