using DominatorHouseCore.Command;
using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Linq;
using System.Windows;

namespace DominatorHouseCore.ViewModel
{
    public class MediaGeneratorViewModel : BindableBase
    {
        public MediaGeneratorViewModel()
        {
            BrowseCommand = new BaseCommand<object>((sender) => true, BrowseExecute);
            CopyCommand = new BaseCommand<object>((sender) => true, CopyExecute);
         
        }

        private void BrowseExecute(object sender)
        {
            try
            {
                string filters = "Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.dat; *.wmv;|All file |*.*";

                var picPath = FileUtilities.GetImageOrVideo(true, filters);
                if (picPath != null)
                {
                    foreach (var pic in picPath)
                    {
                        if (!LstFile.Any(x => x == pic))
                            LstFile.Add(pic);
                    }

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void CopyExecute(object sender)
        {
            try
            {
                string filePath = sender as string;

                if (!string.IsNullOrEmpty(filePath))
                {
                    Clipboard.SetText(filePath);
                    ToasterNotification.ShowSuccess("File path copied");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public ICommand BrowseCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        private ObservableCollection<string> _lstFile = new ObservableCollection<string>();

        public ObservableCollection<string> LstFile
        {
            get
            {
                return _lstFile;
            }
            set
            {
                if (_lstFile == value)
                    return;
                _lstFile = value;
                OnPropertyChanged(nameof(LstFile));
            }
        }
    }
}
