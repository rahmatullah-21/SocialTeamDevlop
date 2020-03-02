using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel
{
    public class NetworksActivityCustomizeViewModel : BindableBase
    {
        public NetworksActivityCustomizeViewModel()
        {
            ChangeActivityStatusCmd = new DelegateCommand<NetworkCustomizeActivityTypeModel>(ChangeActivityStatus);
            SaveCommand = new DelegateCommand(Save);
        }

        public NetworksActivityCustomizeViewModel(NetworksActivityCustomizeModel model) : this()
        {
            Model = model;
        }

        NetworksActivityCustomizeModel _model;
        public NetworksActivityCustomizeModel Model
        {
            get => _model;
            set { SetProperty(ref _model, value); }
        }

        public ICommand SaveCommand { get; }
        public bool IsSaved { get; set; }
        public DelegateCommand<NetworkCustomizeActivityTypeModel> ChangeActivityStatusCmd { get; }

        Dictionary<SocialNetworks, ActivityType> LastChanged = new Dictionary<SocialNetworks, ActivityType>();
        private void ChangeActivityStatus(NetworkCustomizeActivityTypeModel currentDataContext)
        {
            var getOne = Model.NetworksActListCollection.ToList().FirstOrDefault(x => x.SocialNetwork == currentDataContext.Network);

            if (currentDataContext.IsSelected)
            {
                if (getOne.NetworkActivityTypeModelCollections.Count(x => x.IsSelected) > 6)
                {
                    var lastOne = getOne.NetworkActivityTypeModelCollections.Last(x => x.IsSelected && x.Title != currentDataContext.Title);
                    lastOne.IsSelected = false;
                    ToasterNotification.ShowInfomation("You're allowed to select any 6 activities only.");
                }
            }
            else if (getOne.NetworkActivityTypeModelCollections.Count(x => x.IsSelected) == 0)
            {
                currentDataContext.IsSelected = true;
                ToasterNotification.ShowInfomation("Atleast anyone activity should be selected");
            }
            
        }

        void AddToDict(SocialNetworks net, ActivityType act)
        {
            if (!LastChanged.ContainsKey(net))
                LastChanged.Add(net, act);
            else
                LastChanged[net] = act;
        }

        void Save()
        {
            var binFileHelper = CommonServiceLocator.ServiceLocator.Current.GetInstance<IBinFileHelper>();
            if (binFileHelper.SaveAutoActivityCustomized(Model))
            {
                ToasterNotification.ShowSuccess("LangKeySucceededInSaving".FromResourceDictionary());
                IsSaved = true;
            }
            else
                ToasterNotification.ShowError("LangKeyOopsAnErrorOccured".FromResourceDictionary());

        }


    }
}
