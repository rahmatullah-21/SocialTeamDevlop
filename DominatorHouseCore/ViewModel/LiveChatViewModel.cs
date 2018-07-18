using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.ViewModel
{
    public class LiveChatViewModel : BindableBase
    {
        private LiveChatModel _liveChatModel = new LiveChatModel();
       

        public LiveChatModel LiveChatModel
        {
            get
            {
                return _liveChatModel;
            }
            set
            {
                if (value == _liveChatModel)
                    return;
                SetProperty(ref _liveChatModel, value);
            }
        }


        public List<DominatorAccountModel> lstAccountModel { get; set; } = new List<DominatorAccountModel>();

       
    }
}
