using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Interfaces
{
    public interface IDestinationSelectors
    {
        void GetGroupsPair(string accountId, string accountName, ObservableCollection<PublisherCreateDestinationSelectModel> destinationDetails, PublisherCreateDestinationModel destination);

        void GetPagesOrBoardsPair(string accountId, string accountName, ObservableCollection<PublisherCreateDestinationSelectModel> destinationDetails, PublisherCreateDestinationModel destination);

        bool IsGroupsAvailables { get; set; }

        bool IsPagesOrBoardsAvailable { get; set; }

        Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList);

        Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList);

    }
}