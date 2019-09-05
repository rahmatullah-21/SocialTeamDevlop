using System;
using System.Collections.Generic;

namespace DominatorHouseCore.Interfaces
{
    public interface IPostScraperConstants
    {
        List<string> LstRunningAccountsAds { get; set; }

        List<string> LstRunningAccountsLcs { get; set; }

        DateTime LastLcsJobTime { get; set; }
    }
}