using DominatorHouseCore.Models;
using System.Collections.Generic;

namespace DominatorHouseCore.Interfaces
{
    public interface ISocialBrowserManager : IBrowserManager
    {
        void CloseBrowser(DominatorAccountModel account);

        void ExpandGoogleImagesFromLink(string url, ref string title);

        string GetGoogleImages();

        bool HasMoreResults();

    }
}
