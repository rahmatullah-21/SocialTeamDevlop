using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DominatorHouseCore.Models;

namespace DominatorHouse.Utilities
{
    public class ScrapAdsDetails
    {
        public string AccountId { get; set; }

        public DominatorAccountModel account { get; set; }

        public ScrapAdsDetails(DominatorAccountModel AccountModel)
        {
            account = AccountModel;
        }

        public static ConcurrentDictionary<string, CancellationTokenSource> AccountUpdatesCancellationToken { get; set; }
            = new ConcurrentDictionary<string, CancellationTokenSource>();


        public async Task StartAdScarperAsync(ActionBlock<ScrapAdsDetails> adsActionBlock)
        {

            //try
            //{
            //    var cancellationTokenSource = AccountUpdatesCancellationToken.GetOrAdd(account.AccountId, token => new CancellationTokenSource());

            //    if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork))
            //        return;

            //    var accountFactory = SocinatorInitialize
            //        .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
            //        .GetNetworkCoreFactory().AdScraperFactory;

            //    var asyncAccount = accountFactory as IAdScraperFactory;

            //    if (asyncAccount == null)
            //        return;

            //    try
            //    {
            //        cancellationTokenSource.Token.ThrowIfCancellationRequested();

            //        var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

            //        if (!checkResult)
            //            return;

            //        cancellationTokenSource.Token.ThrowIfCancellationRequested();

            //        await asyncAccount.ScrapeAdsAsync(account, cancellationTokenSource.Token);

            //        var dateTime = DateTime.Now;

            //        if (account.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook ||
            //        account.AccountBaseModel.AccountNetwork == SocialNetworks.Reddit)
            //            dateTime = DateTime.Now.AddMinutes(15);

            //        else if (account.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram)
            //            dateTime = DateTime.Now.AddMinutes(5);

            //        JobManager.AddJob(async () =>
            //        {
            //            try
            //            {
            //                await adsActionBlock.SendAsync(new ScrapAdsDetails(account));
            //            }
            //            catch (ArgumentException ex)
            //            {
            //                ex.DebugLog();
            //            }
            //            catch (Exception ex)
            //            {
            //                ex.DebugLog();
            //            }
            //        }, s => s.ToRunOnceAt(dateTime));

            //    }
            //    catch (OperationCanceledException ex)
            //    {
            //        ex.DebugLog("Cancellation Requested!");
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.DebugLog();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

    }
}