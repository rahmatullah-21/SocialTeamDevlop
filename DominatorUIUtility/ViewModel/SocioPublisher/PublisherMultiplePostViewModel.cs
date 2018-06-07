using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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

            var objPublisherPostlistModel = new PublisherPostlistModel();
            objPublisherPostlistModel.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");
            objPublisherPostlistModel.InitializePostData();
            var objPublisherPostlistModel2 = new PublisherPostlistModel();
            objPublisherPostlistModel2.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");
            objPublisherPostlistModel2.InitializePostData();

            PublisherPostlistModels.Add(objPublisherPostlistModel);
            PublisherPostlistModels.Add(objPublisherPostlistModel2);
            PostListsCollectionView = CollectionViewSource.GetDefaultView(_publisherPostlistModels);
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





        #endregion

    }

}
