using System.Collections.Generic;
using System.Threading.Tasks;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IDestinationSelectors
    {
        List<KeyValuePair<string, string>> GetGroupsPair(string accountId,string accountName);

        List<KeyValuePair<string, string>> GetPagesOrBoardsPair(string accountId, string accountName);

        bool IsGroupsAvailables { get; set; }

        bool IsPagesOrBoardsAvailable { get; set; }
    }
}