using System.Windows;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherMonitorFolderModel : BindableBase
    {
        private string _folderPath = string.Empty;
        [ProtoMember(1)]
        public string FolderPath
        {
            get
            {
                return _folderPath;
            }
            set
            {
                if (value == _folderPath)
                    return;
                SetProperty(ref _folderPath, value);
            }
        }
        private string _folderTemplate = Application.Current.FindResource("DhlangFolderPathTemplate").ToString();
        [ProtoMember(2)]
        public string FolderTemplate
        {
            get
            {
                return _folderTemplate;
            }
            set
            {
                if (value == _folderTemplate)
                    return;
                SetProperty(ref _folderTemplate, value);
            }
        }
        private string _buttonContent = "Save to List";
        [ProtoIgnore]
        public string ButtonContent
        {
            get
            {
                return _buttonContent;
            }
            set
            {
                if (value == _buttonContent)
                    return;
                SetProperty(ref _buttonContent, value);
            }
        }
    }
}