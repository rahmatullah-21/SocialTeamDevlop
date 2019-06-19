using CefSharp;
using System.Collections.Generic;

namespace EmbeddedBrowser.BrowserHelper
{
    public class TempFileDialogHandler : IDialogHandler
    {
        MahApps.Metro.Controls.MetroWindow _mainForm;

        private string _path;

        private List<string> _pathList;

        public TempFileDialogHandler(MahApps.Metro.Controls.MetroWindow form, string filePath,
            List<string> pathList = null)
        {
            _mainForm = form;
            _path = filePath;
            _pathList = pathList;
        }
        public bool OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            if (_pathList != null && _pathList.Count > 0)
                callback.Continue(selectedAcceptFilter, _pathList);
            else
                callback.Continue(selectedAcceptFilter, new List<string> { _path });
            return true;
        }
    }
}
