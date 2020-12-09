#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public interface IMainViewModel : IDisposable
    {
        void AddNetwork(SocialNetworks socialNetwork);
        void SetActiveNetwork(SocialNetworks social);
        AccessorStrategies Strategies { get; }

        ObservableCollection<DominatorAccountModel> AccountList { get; set; }

        KeyValuePair<int, int> ScreenResolution { get; set; }
    }
}