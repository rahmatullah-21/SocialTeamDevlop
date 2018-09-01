using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherMultiplePostViewModel : BindableBase
    {

        public PublisherMultiplePostViewModel()
        {
            LstPostDetailsModel = new ObservableCollection<PostDetailsModel>();
            CreateNewPost = new BaseCommand<object>(CanExecuteCreateNewPost, ExecuteCreateNewPost);
            ImportFromCsvCommand = new BaseCommand<object>(ImportFromCsvCanExecute, ImportFromCsvExecute);
            DeletePostCommand = new BaseCommand<object>(DeletePostCanExecute, DeletePostExecute);

            BindingOperations.EnableCollectionSynchronization(LstPostDetailsModel, lockObject);
        }

        #region Command

        public ICommand CreateNewPost { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }
        public ICommand DeletePostCommand { get; set; }

        #endregion

        #region Properties

        private static object lockObject = new object();

        private ObservableCollection<PostDetailsModel> _lstPostDetailsModel;

        public ObservableCollection<PostDetailsModel> LstPostDetailsModel
        {
            get
            {
                return _lstPostDetailsModel;
            }
            set
            {
                if (_lstPostDetailsModel == value)
                    return;
                _lstPostDetailsModel = value;
                SetProperty(ref _lstPostDetailsModel, value);
            }
        }

        private ICollectionView _postListsCollectionView;

        public ICollectionView PostListsCollectionView
        {
            get
            {
                return _postListsCollectionView;
            }
            set
            {
                if (_postListsCollectionView == value)
                    return;
                _postListsCollectionView = value;
                SetProperty(ref _postListsCollectionView, value);
            }
        }


        #endregion

        #region Create New Post

        public bool CanExecuteCreateNewPost(object sender) => true;

        public void ExecuteCreateNewPost(object sender)
        {
            PostDetailsModel postDetailsModel = new PostDetailsModel
            {
                CreatedDateTime = DateTime.Now,
                PostDetailsId = Utilities.GetGuid()
            };

            LstPostDetailsModel.Add(postDetailsModel);

            PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.LstPostDetailsModels = LstPostDetailsModel;
        }


        #endregion

        #region Import From Csv
        private bool ImportFromCsvCanExecute(object sender) => true;

        private void ImportFromCsvExecute(object sender)
        {
            var listPostDetailsModel = FileUtilities.FileBrowseAndReader();
            var separator = ConstantVariable.Separator;

            ThreadFactory.Instance.Start(() =>
            {

                // Iterate selected file name
                listPostDetailsModel.ForEach(x =>
                {
                    PostDetailsModel postDetailsModel = new PostDetailsModel();
                    try
                    {
                        // Split the file details
                        var allData = x.Split('\t');

                        postDetailsModel.PostDescription = allData[0];

                        // Media list

                        #region Medialist

                        var mediaDetails = allData.Length > 1 ? allData[1] : string.Empty;

                        var mediaUrl = Regex.Split(mediaDetails, separator).ToList();
                        mediaUrl.ForEach(media =>
                        {
                            if (File.Exists(media))
                                postDetailsModel.MediaViewer.MediaList.Add(media);
                        });

                        #endregion

                        // Title
                        postDetailsModel.PublisherInstagramTitle = allData.Length > 2 ? allData[2] : string.Empty;

                        // Source url
                        postDetailsModel.PdSourceUrl = allData.Length > 3 ? allData[3] : string.Empty;

                        // Facebook Sell post details

                        #region FdSell

                        var FdSellDetails = allData.Length > 4 ? allData[4] : string.Empty;

                        var Fdsell = Regex.Split(FdSellDetails, separator);

                        if (string.Compare(Fdsell[0], "Yes", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                            string.Compare(Fdsell[0], "Y", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                            string.Compare(Fdsell[0], "True", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            postDetailsModel.IsFdSellPost = true;
                            postDetailsModel.FdSellProductTitle = Fdsell[1];
                            postDetailsModel.FdSellPrice = double.Parse(Fdsell[2]);
                            postDetailsModel.FdSellLocation = Fdsell[3];
                        }

                        #endregion
                        // Created date
                        postDetailsModel.CreatedDateTime = DateTime.Now;

                        // Post id
                        postDetailsModel.PostDetailsId = Utilities.GetGuid();

                        // Add to Collections
                        //postDetails.Add(postDetailsModel);
                        Application.Current.Dispatcher.Invoke(() => LstPostDetailsModel.Add(postDetailsModel));
                        Thread.Sleep(50);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                });
            });
        }
        #endregion

        #region Delete Post

        private bool DeletePostCanExecute(object sender) => true;

        private void DeletePostExecute(object sender)
        {
            try
            {
                var content = sender as string;
                if (content == "DeleteAll")
                    LstPostDetailsModel.Clear();
                else
                {
                    try
                    {
                        var postToDelete = sender as PostDetailsModel;
                        LstPostDetailsModel.Remove(postToDelete);
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
        }
        #endregion
    }

}
