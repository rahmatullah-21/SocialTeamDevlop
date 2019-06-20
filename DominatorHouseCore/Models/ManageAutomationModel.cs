using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageAutomationModel : BindableBase
    {
        private bool _isNetworkSelcted;
        [ProtoMember(1)]
        public bool IsNetworkSelcted
        {
            get
            {
                return _isNetworkSelcted;
            }
            set
            {
                if (value == _isNetworkSelcted)
                    return;
                SetProperty(ref _isNetworkSelcted, value);
                try
                {
                    var softwareSetting = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
                    if (SocialNetwork == SocialNetworks.Social.ToString())
                    {
                        softwareSetting.Settings.NetworkBrowserAutomationList.ForEach(x =>
                            {
                                x.IsNetworkSelcted = value;
                            });
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        private string _socialNetwork;
        [ProtoMember(2)]
        public string SocialNetwork
        {
            get
            {
                return _socialNetwork;
            }
            set
            {
                if (value == _socialNetwork)
                    return;
                SetProperty(ref _socialNetwork, value);
            }
        }
    }
}
