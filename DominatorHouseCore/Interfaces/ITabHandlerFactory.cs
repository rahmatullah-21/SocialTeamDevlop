using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface ITabHandlerFactory
    {
        List<TabItemTemplates> NetworkTabs { get; set; }

        string NetworkName { get; set; }

        void UpdateAccountCustomControl(SocialNetworks networks);
    }
}