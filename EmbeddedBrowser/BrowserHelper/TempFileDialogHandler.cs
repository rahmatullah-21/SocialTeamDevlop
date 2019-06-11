using CefSharp;
using System.Collections.Generic;

namespace EmbeddedBrowser.BrowserHelper
{
    public class TempFileDialogHandler : IDialogHandler
    {
        MahApps.Metro.Controls.MetroWindow _mainForm;

        private string _path;
        public TempFileDialogHandler(MahApps.Metro.Controls.MetroWindow form, string filePath)
        {
            _mainForm = form;
            _path = filePath;
        }
        public bool OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            callback.Continue(selectedAcceptFilter, new List<string> { _path });
            return true;
        }
    }
}
