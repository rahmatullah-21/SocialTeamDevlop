using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorUIUtility.ViewModel.Startup;
using System;
using System.Windows;

namespace DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig
{


    public interface IInstagramVisibilityModel
    {
        Visibility InstagramElementsVisibility { get; set; }
    }
  


    public class ElementsVisibility
    {
        public static void NetworkElementsVisibilty(dynamic visibilityModel)
        {
            try
            {
                var nw = ServiceLocator.Current.TryResolve<ISelectActivityViewModel>().SelectedNetwork;
                var network = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), nw);
                switch (network)
                {
                    case SocialNetworks.Instagram:
                        visibilityModel.InstagramElementsVisibility = Visibility.Visible;
                        break;
                }
            }
            catch{ }
        }
    }
}
