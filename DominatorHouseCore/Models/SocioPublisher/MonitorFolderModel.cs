using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{

    [ProtoContract]
    public class MonitorFolderModel
    {
        public string FolderId { get; set; }

        public string FolderPath { get; set; }

        public string FilePath { get; set; }

        public string FileGuid { get; set; }

        public string PostTemplate { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileTitle { get; set; }

        public string FileCreationDate { get; set; }

        public string FileTags { get; set; }

        public string FileSubject { get; set; }

        public string FileAuthor { get; set; }

        public string FileComment { get; set; }

        public string PostPriority { get; set; }

        public DateTime PostAddedDateTime { get; set; }

        public string FullPostDetails { get; set; }
    }

   
}