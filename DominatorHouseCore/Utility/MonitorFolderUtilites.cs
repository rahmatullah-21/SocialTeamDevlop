using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using Shell32;
using System.Threading;

namespace DominatorHouseCore.Utility
{
    public class MonitorFolderUtilites
    {
        /// <summary>
        /// Get the file information for the file path
        /// </summary>
        /// <param name="filePath">Pass the file name</param>
        /// <returns> Return as detailed info file<see cref="DominatorHouseCore.Models.SocioPublisher.DetailedFileInfo"/></returns>
        private static IEnumerable<DetailedFileInfo> GetDetailedFileInfo(string filePath)
        {
            var lstDetailedFileInfo = new List<DetailedFileInfo>();

            if (filePath.Length <= 0)
                return lstDetailedFileInfo;

            try
            {
                // Folder All_Directory = objShellClass.NameSpace(System.IO.Path.GetDirectoryName(FilePath));
                // Get the proper namespace
                var allDirectory = GetShell32NameSpaceFolder(Path.GetDirectoryName(filePath));

                // Get the file Items
                var folderItem = allDirectory.ParseName(Path.GetFileName(filePath));

                for (var index = 0; index < 30; index++)
                {
                    try
                    {
                        // get the details of a file 
                        var details = allDirectory.GetDetailsOf(folderItem, index);

                        // Assigned to detailed file info
                        var objDetailedFileInfo = new DetailedFileInfo(index, details);

                        lstDetailedFileInfo.Add(objDetailedFileInfo);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return lstDetailedFileInfo;
        }

        /// <summary>
        /// To Get the folder details 
        /// </summary>
        /// <param name="folderpath">Folder path</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="postTemplate">Post template for monitor folder</param>
        /// <param name="postDetailsModel">Post Details-> from here we are getting post's general settings</param>
        /// <param name="cancellationTokenSource">Cancellation Token</param>
        /// <param name="maximumPostLimitToStore">Count for maximum posts for a post list</param>
        /// <param name="campaignName">Campaign Name</param>
        public void GetFoldersFileDetails(string folderpath, string campaignId, string postTemplate, PostDetailsModel postDetailsModel, CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        {
            try
            {
                var postlists = new List<PublisherPostlistModel>();

                var publisherInitialize = PublisherInitialize.GetInstance;

                // Get the folder details
                var foldersFiles = Directory.EnumerateFiles(folderpath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4") || s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".wmv")).ToList();

                // Get the campaigns Post details 
                var campaignDetails = PostlistFileManager.GetAll(campaignId);

                // Filter only monitor folder details
                var monitorFolderFiles = campaignDetails
                    .Where(x => x.PostSource == PostSource.MonitorFolderPost).ToList();

                var isNoUniqueTitleToasterNotified = false;

                var usedMonitorFolderTitle = monitorFolderFiles.Where(x => x.PublisherInstagramTitle != null).Select(x => x.PublisherInstagramTitle).ToList();

                var postTitles = Regex.Split(postDetailsModel.PublisherInstagramTitle, "\r\n").ToList();

                var givenPostTitle = new List<string>();

                postTitles.ForEach(title =>
                {
                    givenPostTitle.Add(title.Trim());
                });

                // If any files are deleted, then remove from post list
                if (foldersFiles.Count < monitorFolderFiles.Count)
                {
                    // Gather not available post list
                    var notAvailableFiles = monitorFolderFiles.Select(x => x.MonitorFilePath).Except(foldersFiles).ToList();

                    // Iterate One by one and remove
                    notAvailableFiles.ForEach(filePath =>
                    {
                        var post = monitorFolderFiles.FirstOrDefault(x => x.MonitorFilePath == filePath);
                        if (post?.LstPublishedPostDetailsModels.Count <= 0)
                            PostlistFileManager.Delete(post.CampaignId, x => x.PostId == post.PostId);
                    });

                    // Update the post count
                    publisherInitialize.UpdatePostCounts(campaignId);
                }

                var mediaUtilites = new MediaUtilites();

                // Calculate the post expire date time
                DateTime? expireDate = null;
                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsExpireDate)
                    expireDate = postDetailsModel.PublisherPostSettings.GeneralPostSettings.ExpireDate;

                // Iterate the files from folder and fetch the neccessary details
                foreach (var file in foldersFiles)
                {
                    try
                    {

                        // ReSharper disable once RedundantAssignment
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
                                postTitle = availablePostTitles[randomNumber];
                            }
                            else
                            {
                                if (!isNoUniqueTitleToasterNotified)
                                {
                                    isNoUniqueTitleToasterNotified = true;
                                    ToasterNotification.ShowInfomation($"No More unique titles are present in {campaignName}!");
                                }
                                postTitle = string.Empty;
                            }
                        }

                        usedMonitorFolderTitle.Add(postTitle);

                        // Generate the post model
                        var publisherPostlistModel = new PublisherPostlistModel
                        {
                            MediaList = new ObservableCollection<string> { file },
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
                            PublisherPostSettings = postDetailsModel.PublisherPostSettings
                        };
                        // Get the file info
                        var fileDetails = GetDetailedFileInfo(file);
                        var monitorFolderModel = new MonitorFolderModel
                        {
                            FolderPath = folderpath,
                            FilePath = file
                        };

                        // Assign the folder's file details to respective values, Which is going with index value or enum value of file 
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
                                // ReSharper disable once RedundantEmptySwitchSection
                                default:
                                    break;
                            }
                        }

                        #endregion

                        // Manipulate the post descriptions
                        publisherPostlistModel.PostDescription = postTemplate.Replace("[FileName]", monitorFolderModel.FileName.Replace(ConstantVariable.VideoToImageConvertFileName, String.Empty))
                            .Replace("[FileType]", monitorFolderModel.FileType)
                            .Replace("[FileAuthor]", monitorFolderModel.FileAuthor)
                            .Replace("[FileTitle]", monitorFolderModel.FileTitle)
                            .Replace("[FileSubject]", monitorFolderModel.FileSubject)
                            .Replace("[FileCreationDate]", monitorFolderModel.FileCreationDate)
                            .Replace("[FileComments]", monitorFolderModel.FileComment)
                            .Replace("[FileTags]", monitorFolderModel.FileTags);
                        if (campaignDetails.Count > 0)
                        {
                            var post = campaignDetails.FirstOrDefault(x =>
                                x.PostSource == PostSource.MonitorFolderPost && x.MonitorFilePath == file);
                            if (post != null)
                                post.PostDescription = publisherPostlistModel.PostDescription;
                        }

                        // Check whether Cancellation Requested or not
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        // Add the current post to post list
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
                }
                if (campaignDetails.Count > 0)
                    PostlistFileManager.UpdatePostlists(campaignId, campaignDetails);
                // Check whether need to readd the post 
                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsReaddCount)
                {
                    try
                    {
                        var duplicatedPostlist = new List<PublisherPostlistModel>();

                        // Iterate the post lists 
                        foreach (var post in postlists)
                        {
                            try
                            {
                                // Iterate post with readd count for achevie x time post
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
                        // Check whether Cancellation Requested or not
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        // Add the current post to post list
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

                // Get the current campaign Count 
                campaignDetails = PostlistFileManager.GetAll(campaignId);

                // Get the available post counts
                var postCount = maximumPostLimitToStore - campaignDetails.Count;

                if (postCount > 0)
                {
                    // Take need to posts from postlist 
                    var neededPostLists = postlists.Take(postCount).ToList();
                    PostlistFileManager.AddRange(campaignId, neededPostLists);

                    // Update the current post count
                    publisherInitialize.UpdatePostCounts(campaignId);
                }
                else
                {
                    // Inform the maximum post has reached via Toaster notification
                    ToasterNotification.ShowInfomation($"Maximum Postlist Reached: {campaignName} already have {maximumPostLimitToStore}+ posts in postlist!");
                }
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

        /// <summary>
        /// Get the folder details
        /// </summary>
        /// <param name="folder">Directory details</param>
        /// <returns>return as Shell 32 folder details <see cref="Shell32.Folder"/></returns>
        private static Folder GetShell32NameSpaceFolder(object folder)
        {
            // Get the program Identifier details
            var shellAppType = Type.GetTypeFromProgID("Shell.Application");

            // Initializer for current program 
            var shell = Activator.CreateInstance(shellAppType);

            // Get the proper folder details
            return (Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new[] { folder });
        }

    }
}