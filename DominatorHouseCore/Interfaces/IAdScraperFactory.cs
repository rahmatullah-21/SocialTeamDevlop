using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    public interface IAdScraperFactory
    {
        Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token);

        Task ScrapeAdsAsync(DominatorAccountModel accountModel, CancellationToken token);
    }
}
