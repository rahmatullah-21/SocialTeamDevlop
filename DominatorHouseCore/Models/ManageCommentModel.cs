using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageCommentModel : BindableBase
    {

        public ManageCommentModel()
        {
            CommentId = Utilities.GetGuid();
        }
        private string _commentId;

        public string CommentId
        {
            get
            {
                return _commentId;
            }
            set
            {
                if (value == _commentId)
                    return;
                SetProperty(ref _commentId, value);
            }
        }
        private string _commentText;

        public string CommentText
        {
            get
            {
                return _commentText;
            }
            set
            {
                if (value == _commentText)
                    return;
                SetProperty(ref _commentText, value);
            }
        }
        private string _filterText;

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (value == _filterText)
                    return;
                SetProperty(ref _filterText, value);
            }
        }

        private ObservableCollection<QueryContent> _selectedQuery = new ObservableCollection<QueryContent>();

        public ObservableCollection<QueryContent> SelectedQuery
        {
            get { return _selectedQuery; }
            set { SetProperty(ref _selectedQuery, value); }
        }
        private ObservableCollection<QueryContent> _lstQueries = new ObservableCollection<QueryContent>();

        public ObservableCollection<QueryContent> LstQueries
        {
            get { return _lstQueries; }
            set { SetProperty(ref _lstQueries, value); }
        }
        

    }

    [ProtoContract]
    public class QueryContent : BindableBase
    {

        private QueryInfo _content;
        /// <summary>
        /// Provide the content
        /// </summary>
        [ProtoMember(1)]
        public QueryInfo Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != null && value == _content)
                    return;
                SetProperty(ref _content, value);

            }
        }


        private bool _isContentSelected;
        /// <summary>
        /// IsContentSelected is used to give the status whether the content is selected or not
        /// </summary>
        [ProtoMember(2)]
        public bool IsContentSelected
        {
            get
            {
                return _isContentSelected;
            }
            set
            {
                if (value == _isContentSelected)
                    return;
                SetProperty(ref _isContentSelected, value);
            }
        }

    }

}
