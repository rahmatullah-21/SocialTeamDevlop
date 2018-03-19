using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.ViewModel
{
    public class LiveChatViewModel:BindableBase
    {
        private LiveChatModel _liveChatModel=new LiveChatModel();

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

    }
}
