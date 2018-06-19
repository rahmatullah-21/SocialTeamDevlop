using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDirectPostsViewModel : BindableBase
    {
        private PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;
        #region Constructor

        public PublisherDirectPostsViewModel()
        {
            MultiplePostCommand = new BaseCommand<object>(CanExecuteMultiPost, ExecuteMultiPost);
            ImportFromCsvCommand = new BaseCommand<object>(ImportFromCsvCanExecute, ImportFromCsvExecute);
            SearchCommand = new BaseCommand<object>(SearchCanExecute, SearchExecute);
        }



        public PublisherDirectPostsViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            this.tabItemsControl = tabItemsControl;
            PostDetailsModel = tabItemsControl.PostDetailsModel;
        }

        #endregion

        #region Properties

        public ICommand MultiplePostCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        private PostDetailsModel _postDetailsModel = new PostDetailsModel();

        public PostDetailsModel PostDetailsModel
        {
            get
            {
                return _postDetailsModel;
            }
            set
            {
                if (_postDetailsModel == value)
                    return;
                _postDetailsModel = value;
                OnPropertyChanged(nameof(PostDetailsModel));
            }
        }


        private bool _isBool = true;

        public bool IsBool
        {
            get
            {
                return _isBool;
            }
            set
            {
                if (IsBool == value)
                    return;
                _isBool = value;
                OnPropertyChanged(nameof(IsBool));
            }
        }


        private PublisherMediaViewerModel _publisherMultipleImagesMediaViewerModel = new PublisherMediaViewerModel();

        public PublisherMediaViewerModel PublisherMultipleImagesMediaViewerModel
        {
            get
            {
                return _publisherMultipleImagesMediaViewerModel;
            }
            set
            {
                if (_publisherMultipleImagesMediaViewerModel == value)
                    return;
                _publisherMultipleImagesMediaViewerModel = value;
                OnPropertyChanged(nameof(PublisherMultipleImagesMediaViewerModel));
            }
        }

        #endregion

        #region Methods

        public bool CanExecuteMultiPost(object sender) => true;

        public void ExecuteMultiPost(object sender)
        {
            try
            {
                var publisherMultiplePost = new PublisherMultiplePost();
                Dialog dialog = new Dialog();
                var window = dialog.GetMetroWindow(publisherMultiplePost, "Multiple Post");
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SearchCanExecute(object sender) => true;

        private void SearchExecute(object sender)
        {

            var mediaViewer = (MediaViewer)sender;

            PostDetailsModel.MediaList = new ObservableCollection<string>(ImageExtracter.ExtractImageUrls(PostDetailsModel.ImagesUrl));
            mediaViewer.MediaList = PostDetailsModel.MediaList;

            mediaViewer?.Initialize();

        }
        private bool ImportFromCsvCanExecute(object sender) => true;

        private void ImportFromCsvExecute(object sender)
        {
            var listPostDetailsModel = FileUtilities.FileBrowseAndReader();
            var separator = ConstantVariable.Separator;
            ObservableCollection<PostDetailsModel> postDetails = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.LstPostDetailsModels;
            listPostDetailsModel.ForEach(x =>
            {
                PostDetailsModel postDetailsModel = new PostDetailsModel();
                try
                {
                    var allData = x.Split(',');
                    postDetailsModel.PostDescription = allData[0];

                    #region Medialist

                    var mediaUrl = Regex.Split(allData[1], separator).ToList();
                    mediaUrl.ForEach(media =>
                    {
                        if (File.Exists(media))
                            postDetailsModel.MediaViewer.MediaList.Add(media);

                    });

                    #endregion

                    postDetailsModel.PublisherInstagramTitle = allData[2];
                    postDetailsModel.PublisherInstagramTitle = allData[3];

                    #region FdSell

                    var Fdsell = Regex.Split(allData[4], separator);
                    if (Fdsell[0] == "IsEnable")
                    {
                        postDetailsModel.IsFdSellPost = true;
                        postDetailsModel.FdSellProductTitle = Fdsell[1];
                        postDetailsModel.FdSellPrice = double.Parse(Fdsell[2]);
                        postDetailsModel.FdSellLocation = Fdsell[3];
                    }
                    #endregion
                    postDetails.Add(postDetailsModel);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            });
            if (postDetails?.Count!=0)
            {
                try
                {
                    var publisherMultiplePost = new PublisherMultiplePost(postDetails);
                    Dialog dialog = new Dialog();
                    var window = dialog.GetMetroWindow(publisherMultiplePost, "Multiple Post");
                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                } 
            }
        }
        #endregion

    }


}