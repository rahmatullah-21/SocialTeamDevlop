using CefSharp;

namespace EmbeddedBrowser.BrowserHelper
{
    internal class MenuHandler : IContextMenuHandler
    {
        private const int Refresh = 1;
        private const int Back = 2;
        private const int Forward = 3;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            model?.Clear();
            //Add new custom menu items
            //model.AddItem((CefMenuCommand)Refresh, "Refresh");
            //model.AddItem((CefMenuCommand)Back, "Back");
            //model.AddItem((CefMenuCommand)Forward, "Forward");
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int) commandId == Refresh) browser.Reload();
            if ((int) commandId == Back) browser.GoBack();
            if ((int) commandId == Forward) browser.GoForward();

            return false;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}