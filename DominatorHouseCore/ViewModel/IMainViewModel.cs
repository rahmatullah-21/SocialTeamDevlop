using DominatorHouse.Window;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace DominatorHouseCore.ViewModel
{
    public interface IMainViewModel : IDisposable
    {
        AccessorStrategies Strategies { get; }

        void SetActiveNetwork(SocialNetworks social);

        void AddNetwork(SocialNetworks socialNetwork);

        KeyValuePair<int, int> ScreenResolution { get; set; }

        ObservableCollection<DominatorAccountModel> AccountList { get; set; }

        WindowModel CrucialWindowModel { get; set; }
    }
}
