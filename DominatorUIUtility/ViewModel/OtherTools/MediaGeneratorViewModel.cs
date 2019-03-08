using DominatorHouseCore;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            CopyCmd = new DelegateCommand<object>(Copy);
            LstFile = new ObservableCollection<string>();
        }

        private void Copy(object filePaths)
        {
            try
            {
                StringBuilder filesPath = new StringBuilder();
                if (filePaths != null)
                {
                    var data = filePaths as IEnumerable<object>;
                    if (!data.Any())
                    {
                        ToasterNotification.ShowWarning("Please select atleast one path to copy.");
                        return;
                    }
                    data.ForEach(x =>
                    {
                        filesPath.Append(x.ToString());
                        filesPath.AppendLine();
                    });
                    filesPath.Remove(filesPath.Length - 1, 1);
                    Clipboard.SetData(DataFormats.Text, filesPath.ToString());
                    ToasterNotification.ShowSuccess("Files Path copied");
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
