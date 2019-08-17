using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Windows.Input;

namespace DominatorHouseCore.ViewModel
{
    public interface IMainViewModel : IDisposable
    {
        void AddNetwork(SocialNetworks socialNetwork);
        void SetActiveNetwork(SocialNetworks social);
        AccessorStrategies Strategies { get; }
    }
}
