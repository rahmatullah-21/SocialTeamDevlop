using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DominatorHouseCore.ViewModel
{
    public interface IMainViewModel : IDisposable
    {
        void AddNetwork(SocialNetworks socialNetwork);
        void SetActiveNetwork(SocialNetworks social);
        AccessorStrategies Strategies { get; }

        ObservableCollection<DominatorAccountModel> AccountList { get; set; }
    }
}
