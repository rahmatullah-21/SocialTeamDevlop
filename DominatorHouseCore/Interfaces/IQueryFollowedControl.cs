using DominatorHouseCore.Models;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.Interfaces
{
    public interface IQueryFollowedControl
    {
        void AssignReportDetailsToModel(ObservableCollection<object> FollowRepoertList, CampaignDetails Campaign);
    }
}
