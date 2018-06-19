using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using DominatorHouseCore.Command;
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
            //var objPublisherPostlistModel = new PublisherPostlistModel();
            //objPublisherPostlistModel.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");
            //objPublisherPostlistModel.InitializePostData();
            //PublisherPostlistModels.Add(objPublisherPostlistModel);         
            PostListsCollectionView = CollectionViewSource.GetDefaultView(LstPostDetailsModel);

            CreateNewPost = new BaseCommand<object>(CanExecuteCreateNewPost, ExecuteCreateNewPost);
        }


        #region Properties

        private ObservableCollection<PostDetailsModel> _lstPostDetailsModel;

        public ObservableCollection<PostDetailsModel> LstPostDetailsModel
        {
            get
            {
                return _lstPostDetailsModel;
            }
            set
            {
                if(_lstPostDetailsModel == value)
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

        public ICommand CreateNewPost { get; set; }

        #endregion


        #region Create New Post

        public bool CanExecuteCreateNewPost(object sender) => true;

        public void ExecuteCreateNewPost(object sender)
        {
            PostDetailsModel postDetailsModel =new PostDetailsModel();
            LstPostDetailsModel.Add(postDetailsModel);
            PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.LstPostDetailsModels = LstPostDetailsModel;
        }

        #endregion
    }

}
