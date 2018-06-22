using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using Shell32;

namespace DominatorHouseCore.Utility
{
    public class MonitorFolderUtilites
    {
        private static IEnumerable<DetailedFileInfo> GetDetailedFileInfo(string filePath)
        {
            var lstDetailedFileInfo = new List<DetailedFileInfo>();

            if (filePath.Length <= 0)
                return lstDetailedFileInfo;

            try
            {
                // Folder All_Directory = objShellClass.NameSpace(System.IO.Path.GetDirectoryName(FilePath));
                var allDirectory = GetShell32NameSpaceFolder(System.IO.Path.GetDirectoryName(filePath));
                var folderItem = allDirectory.ParseName(System.IO.Path.GetFileName(filePath));
                for (var index = 0; index < 30; index++)
                {
                    try
                    {
                        var details = allDirectory.GetDetailsOf(folderItem, index);
                        var objDetailedFileInfo = new DetailedFileInfo(index, details);
                        lstDetailedFileInfo.Add(objDetailedFileInfo);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
            return lstDetailedFileInfo;
        }

        public void GetFoldersFileDetails(string folderpath, string campaignId, string postTemplate)
        {
            var postlists = new List<PublisherPostlistModel>();

            var foldersFiles = Directory.EnumerateFiles(folderpath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4") || s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".wmv")).ToList();

            var monitorFolderFiles = PostlistFileManager.GetAll(campaignId)
                .Where(x => x.PostSource == PostSource.MonitorFolderPost);

            PostlistFileManager.DeleteSelected(campaignId, monitorFolderFiles.ToList());

            var mediaUtilites = new MediaUtilites();

            foldersFiles.ForEach(file =>
            {
                var publisherPostlistModel = new PublisherPostlistModel
                {
                    MediaList = new ObservableCollection<string> { mediaUtilites.GetThumbnail(file)  },
                    CampaignId = campaignId,
                    CreatedTime = DateTime.Now,
                    ExpiredTime = DateTime.Now.AddYears(1),
                    PostId = Utilities.GetGuid(),
                    PostCategory = PostCategory.OrdinaryPost,
                    PostQueuedStatus = PostQueuedStatus.Pending,
                    PostRunningStatus = PostRunningStatus.Active,
                    PostSource = PostSource.MonitorFolderPost
                };

                var fileDetails = GetDetailedFileInfo(file);
                var monitorFolderModel = new MonitorFolderModel
                {
                    FolderPath = folderpath,
                    FilePath = file
                };

                #region Get from Files

                foreach (var objDetailedFileInfo in fileDetails)
                {
                    switch (objDetailedFileInfo.Id)
                    {
                        case 0:
                            monitorFolderModel.FileName =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value) ?
                                    objDetailedFileInfo.Value : string.Empty;
                            break;
                        case 9:
                            monitorFolderModel.FileType =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value) ?
                                    objDetailedFileInfo.Value : string.Empty;
                            break;
                        case 20:
                            monitorFolderModel.FileAuthor =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value) ?
                                    objDetailedFileInfo.Value : string.Empty;
                            break;
                        case 21:
                            monitorFolderModel.FileTitle =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value) ?
                                    objDetailedFileInfo.Value : string.Empty;
                            break;
                        case 22:
                            monitorFolderModel.FileSubject =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value)
                                    ? objDetailedFileInfo.Value : string.Empty;
                            break;
                        case 4:
                            monitorFolderModel.FileCreationDate =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value) ?
                                    objDetailedFileInfo.Value : string.Empty;
                            break;
                        case 24:
                            monitorFolderModel.FileComment =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value) ?
                                    objDetailedFileInfo.Value : string.Empty;
                            break;
                        case 18:
                            monitorFolderModel.FileTags =
                                !string.IsNullOrEmpty(objDetailedFileInfo.Value) ?
                                    objDetailedFileInfo.Value : string.Empty;
                            break;
                        default:
                            break;
                    }
                }

                #endregion

                publisherPostlistModel.PostDescription = postTemplate.Replace("[FileName]", monitorFolderModel.FileName.Replace(ConstantVariable.VideoToImageConvertFileName,String.Empty))
                    .Replace("[FileType]", monitorFolderModel.FileType)
                    .Replace("[FileAuthor]", monitorFolderModel.FileAuthor)
                    .Replace("[FileTitle]", monitorFolderModel.FileTitle)
                    .Replace("[FileSubject]", monitorFolderModel.FileSubject)
                    .Replace("[FileCreationDate]", monitorFolderModel.FileCreationDate)
                    .Replace("[FileComments]", monitorFolderModel.FileComment)
                    .Replace("[FileTags]", monitorFolderModel.FileTags);

                postlists.Add(publisherPostlistModel);
            });
            PostlistFileManager.AddRange(campaignId, postlists);
        }

        private static Folder GetShell32NameSpaceFolder(object folder)
        {
            var shellAppType = Type.GetTypeFromProgID("Shell.Application");
            var shell = Activator.CreateInstance(shellAppType);
            return (Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new[] { folder });
        }
    }
}