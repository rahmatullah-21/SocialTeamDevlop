using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    /// <summary>
    /// ContentSelectGroup is used in binding where content along with select options such as inside combobox content with checkbox 
    /// </summary>
    [ProtoContract]
    public class ContentSelectGroup : BindableBase
    {
      
        private string _content;
        /// <summary>
        /// Provide the content
        /// </summary>
        [ProtoMember(1)]
        public string Content
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