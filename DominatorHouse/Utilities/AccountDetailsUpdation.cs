using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouse.Utilities
{
    //public class AccountDetailsUpdation
    //{
    //    public string AccountId { get; set; }

    //    public DominatorAccountModel account { get; set; }

    //    public AccountDetailsUpdation(DominatorAccountModel AccountModel)
    //    {
    //        account = AccountModel;
    //    }

    //    public static ConcurrentDictionary<string, CancellationTokenSource> AccountUpdatesCancellationToken { get; set; }
    //        = new ConcurrentDictionary<string, CancellationTokenSource>();



    //    public async Task UpdateAccountAsync()
    //    {

    //        var cancellationTokenSource = AccountUpdatesCancellationToken.GetOrAdd(account.AccountId, token => new CancellationTokenSource());

    //        if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork))
    //            return;

    //        var accountFactory = SocinatorInitialize
    //            .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
    //            .GetNetworkCoreFactory().AccountUpdateFactory;

    //        var asyncAccount = accountFactory as IAccountUpdateFactoryAsync;

    //        if (asyncAccount == null)
    //            return;

    //        try
    //        {
    //            cancellationTokenSource.Token.ThrowIfCancellationRequested();

    //            var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

    //            if (!checkResult)
    //                return;

    //            cancellationTokenSource.Token.ThrowIfCancellationRequested();

    //            await asyncAccount.UpdateDetailsAsync(account, cancellationTokenSource.Token);

    //            new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
    //                .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
    //                .SaveToBinFile();

    //        }
    //        catch (OperationCanceledException ex)
    //        {
    //            ex.DebugLog("Cancellation Requested!");
    //        }
    //        catch (Exception ex)
    //        {
    //            ex.DebugLog();
    //        }
    //    }
    //}
}