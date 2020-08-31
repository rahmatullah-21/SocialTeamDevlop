using System.Collections.Generic;
using CefSharp;
using MahApps.Metro.Controls;

namespace EmbeddedBrowser.BrowserHelper
{
    public class TempFileDialogHandler : IDialogHandler
    {
        private readonly string _path;

        private readonly List<string> _pathList;

        public TempFileDialogHandler(MetroWindow form, string filePath,
            List<string> pathList = null)
        {
            _path = filePath;
            _pathList = pathList;
        }

        public bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode,
            CefFileDialogFlags flags, string title, string defaultFilePath, List<string> acceptFilters,
            int selectedAcceptFilter, IFileDialogCallback callback)
        {
            if (_pathList != null && _pathList.Count > 0)
                callback.Continue(selectedAcceptFilter, _pathList);
            else
                callback.Continue(selectedAcceptFilter, new List<string> {_path});
            return true;
        }
    }
}