using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;

namespace DominatorHouseCore.ViewModel
{
    public interface IMainViewModel : IDisposable
    {
        void AddNetwork(SocialNetworks socialNetwork);
        void SetActiveNetwork(SocialNetworks social);
        AccessorStrategies Strategies { get; }
    }
}
