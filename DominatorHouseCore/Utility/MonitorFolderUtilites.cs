using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using Shell32;
using System.Threading;

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

        public void GetFoldersFileDetails(string folderpath, string campaignId, string postTemplate, PostDetailsModel postDetailsModel, CancellationTokenSource cancellationTokenSource, int notifyCount, string campaignName)
        {
            try
            {
                var postlists = new List<PublisherPostlistModel>();

                var publisherInitialize = PublisherInitialize.GetInstance;

                var foldersFiles = Directory.EnumerateFiles(folderpath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4") || s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".wmv")).ToList();

                var monitorFolderFiles = PostlistFileManager.GetAll(campaignId)
                    .Where(x => x.PostSource == PostSource.MonitorFolderPost).ToList();


                var usedMonitorFolderTitle = monitorFolderFiles.Where(x=> x.PublisherInstagramTitle!=null).Select(x => x.PublisherInstagramTitle).ToList();

                var postTitles = Regex.Split(postDetailsModel.PublisherInstagramTitle, "\r\n").ToList();

                var givenPostTitle = new List<string>();

                postTitles.ForEach(title =>
                {
                    givenPostTitle.Add(title.Trim());
                });


                var postTitle = string.Empty;

                if (postDetailsModel.IsRandomlyPickTitleFromList)
                {
                    var randomNumber = RandomUtilties.GetRandomNumber(givenPostTitle.Count - 1);
                    postTitle = givenPostTitle[randomNumber];
                }

                // if (postDetailsModel.IsRemoveTitleOnceUsed)
                else
                {
                    var availablePostTitles = givenPostTitle.Except(usedMonitorFolderTitle).ToList();

                    if (availablePostTitles.Count > 0)
                    {
                        var randomNumber = RandomUtilties.GetRandomNumber(availablePostTitles.Count - 1);
                        postTitle = givenPostTitle[randomNumber];
                    }
                    else
                    {
                        ToasterNotification.ShowInfomation($"No More unique titles are present in {campaignName}!");                  
                        postTitle =string.Empty;
                    }
                }


                if (foldersFiles.Count < monitorFolderFiles.Count)
                {
                    var notAvailableFiles = monitorFolderFiles.Select(x => x.MonitorFilePath).Except(foldersFiles).ToList();
                    notAvailableFiles.ForEach(filePath =>
                    {
                        var post = monitorFolderFiles.FirstOrDefault(x => x.MonitorFilePath == filePath);
                        if (post?.LstPublishedPostDetailsModels.Count <= 0)
                            PostlistFileManager.Delete(post.CampaignId, x => x.PostId == post.PostId);
                    });
                    publisherInitialize.UpdatePostCounts(campaignId);
                }

                var mediaUtilites = new MediaUtilites();

                DateTime? expireDate = null;

                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsExpireDate)
                    expireDate = postDetailsModel.PublisherPostSettings.GeneralPostSettings.ExpireDate;

                foldersFiles.ForEach(file =>
                {
                    try
                    {
                        var publisherPostlistModel = new PublisherPostlistModel
                        {
                            MediaList = new ObservableCollection<string> { mediaUtilites.GetThumbnail(file) },
                            CampaignId = campaignId,
                            CreatedTime = DateTime.Now,
                            ExpiredTime = expireDate,
                            PostId = Utilities.GetGuid(),
                            PostCategory = PostCategory.OrdinaryPost,
                            PostQueuedStatus = PostQueuedStatus.Pending,
                            PostRunningStatus = PostRunningStatus.Active,
                            PostSource = PostSource.MonitorFolderPost,
                            MonitorFilePath = file,
                            PdSourceUrl = postDetailsModel.PdSourceUrl,
                            PublisherInstagramTitle = postTitle,
                            GeneralPostSettings = postDetailsModel.PublisherPostSettings.GeneralPostSettings,
                            FdPostSettings = postDetailsModel.PublisherPostSettings.FdPostSettings,
                            GdPostSettings = postDetailsModel.PublisherPostSettings.GdPostSettings,
                            TdPostSettings = postDetailsModel.PublisherPostSettings.TdPostSettings,
                            LdPostSettings = postDetailsModel.PublisherPostSettings.LdPostSettings,
                            TumberPostSettings = postDetailsModel.PublisherPostSettings.TumberPostSettings,
                            RedditPostSetting = postDetailsModel.PublisherPostSettings.RedditPostSetting,
                            FdSellLocation = postDetailsModel.FdSellLocation,
                            FdSellPrice = postDetailsModel.FdSellPrice,
                            FdSellProductTitle = postDetailsModel.FdSellProductTitle,
                            IsFdSellPost = postDetailsModel.IsFdSellPost,
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

                        publisherPostlistModel.PostDescription = postTemplate.Replace("[FileName]", monitorFolderModel.FileName.Replace(ConstantVariable.VideoToImageConvertFileName, String.Empty))
                                .Replace("[FileType]", monitorFolderModel.FileType)
                                .Replace("[FileAuthor]", monitorFolderModel.FileAuthor)
                                .Replace("[FileTitle]", monitorFolderModel.FileTitle)
                                .Replace("[FileSubject]", monitorFolderModel.FileSubject)
                                .Replace("[FileCreationDate]", monitorFolderModel.FileCreationDate)
                                .Replace("[FileComments]", monitorFolderModel.FileComment)
                                .Replace("[FileTags]", monitorFolderModel.FileTags);

                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (monitorFolderFiles.All(x => x.MonitorFilePath != file))
                        {
                            postlists.Add(publisherPostlistModel);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Cancellation Requested!");
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var e in ae.InnerExceptions)
                        {
                            if (e is TaskCanceledException || e is OperationCanceledException)
                                throw new OperationCanceledException("Cancellation Requested!");
                            else
                                e.DebugLog(e.StackTrace + e.Message);
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });


                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsReaddCount)
                {
                    try
                    {
                        var duplicatedPostlist = new List<PublisherPostlistModel>();

                        foreach (var post in postlists)
                        {
                            try
                            {
                                for (var readdIndex = 1; readdIndex < postDetailsModel.PublisherPostSettings.GeneralPostSettings.ReaddCount; readdIndex++)
                                {
                                    var newPost = post.DeepClone();
                                    newPost.PostId = Utilities.GetGuid();
                                    duplicatedPostlist.Add(newPost);
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        postlists.AddRange(duplicatedPostlist);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Cancellation Requested!");
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var e in ae.InnerExceptions)
                        {
                            if (e is TaskCanceledException || e is OperationCanceledException)
                                throw new OperationCanceledException("Cancellation Requested!");
                            else
                                e.DebugLog(e.StackTrace + e.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                PostlistFileManager.AddRange(campaignId, postlists);

                publisherInitialize.UpdatePostCounts(campaignId);

            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static Folder GetShell32NameSpaceFolder(object folder)
        {
            var shellAppType = Type.GetTypeFromProgID("Shell.Application");
            var shell = Activator.CreateInstance(shellAppType);
            return (Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new[] { folder });
        }
    }
}