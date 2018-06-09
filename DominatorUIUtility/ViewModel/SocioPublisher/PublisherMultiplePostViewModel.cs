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
            PublisherPostlistModels = new ObservableCollection<PublisherPostlistModel>();
            //var objPublisherPostlistModel = new PublisherPostlistModel();
            //objPublisherPostlistModel.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");
            //objPublisherPostlistModel.InitializePostData();
            //PublisherPostlistModels.Add(objPublisherPostlistModel);         
            PostListsCollectionView = CollectionViewSource.GetDefaultView(_publisherPostlistModels);

            CreateNewPost = new BaseCommand<object>(CanExecuteCreateNewPost, ExecuteCreateNewPost);
        }

        #region Properties

        private ObservableCollection<PublisherPostlistModel> _publisherPostlistModels;

        public ObservableCollection<PublisherPostlistModel> PublisherPostlistModels
        {
            get
            {
                return _publisherPostlistModels;
            }
            set
            {
                if(_publisherPostlistModels == value)
                    return;
                _publisherPostlistModels = value;
                SetProperty(ref _publisherPostlistModels, value);
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
            
        }

        #endregion
    }

}
