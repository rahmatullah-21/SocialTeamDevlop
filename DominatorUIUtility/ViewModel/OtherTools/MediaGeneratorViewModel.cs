using DominatorHouseCore;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.OtherTools
{
    public class MediaGeneratorViewModel : BaseTabViewModel, IOtherToolsViewModel
    {
        public ICommand BrowseCommand { get; }
        public ICommand CopyCmd { get; }
        public ObservableCollection<string> LstFile { get; }

        public MediaGeneratorViewModel() : base("LangKeyMediaGenerator", "MediaGeneratorControlTemplate")
        {
            BrowseCommand = new DelegateCommand(BrowseExecute);
            CopyCmd = new DelegateCommand<string>(Copy);
            LstFile = new ObservableCollection<string>();
        }

        private void Copy(string path)
        {
            try
            {
                var filePath = path;
                if (!string.IsNullOrEmpty(filePath))
                {
                    Clipboard.SetText(filePath);
                    ToasterNotification.ShowSuccess("File Path copied");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BrowseExecute()
        {
            try
            {
                string filters = "Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.dat; *.wmv;|All file |*.*";

                var picPath = FileUtilities.GetImageOrVideo(true, filters);
                if (picPath != null)
                {
                    foreach (var pic in picPath)
                    {
                        if (LstFile.All(x => x != pic))
                            LstFile.Add(pic);
                    }

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
