using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IAdScraperFactory
    {
        Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token);

        Task ScrapeAdsAsync(DominatorAccountModel accountModel, CancellationToken token);
    }
}