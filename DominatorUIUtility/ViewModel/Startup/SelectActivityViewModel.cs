using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.NetworkActivitySetting;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.Startup
{
    //public interface ISelectActivityViewModel
    //{
    //    void SetActivityTypeByNetwork(SocialNetworks network);
    //  }
    //public class SelectActivityViewModel : BindableBase, ISelectActivityViewModel
    //{
       
    //    public void SetActivityTypeByNetwork(SocialNetworks network)
    //    {
    //        SelectActivityModel.LstNetworkActivityType.Clear();
    //        foreach (var name in Enum.GetNames(typeof(ActivityType)))
    //        {
    //            if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(network.ToString()))
    //                SelectActivityModel.LstNetworkActivityType.Add(new ActivityChecked
    //                {
    //                    ActivityType = name
    //                });
    //        }
    //    }
    //}


}
