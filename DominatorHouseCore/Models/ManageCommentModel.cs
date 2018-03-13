using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageCommentModel:BindableBase
    {
        

        private int _serialNo;
        
        public int SerialNo
        {
            get
            {
                return _serialNo;
            }
            set
            {
                if (value == _serialNo)
                    return;
                SetProperty(ref _serialNo, value);
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
       
     
        public ObservableCollection<string> SelectedQuery { get; set; } = new ObservableCollection<string>();
       
       
        public ObservableCollection<ContentSelectGroup> LstQueries { get; set;} = new ObservableCollection<ContentSelectGroup>();


    }

  
   
}
