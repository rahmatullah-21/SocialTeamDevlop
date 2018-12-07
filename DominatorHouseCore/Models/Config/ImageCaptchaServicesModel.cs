using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Config
{
    [ProtoContract]
    public class ImageCaptchaServicesModel : BindableBase
    {
        private string _userName;
        [ProtoMember(1)]
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }
        private string _password;
        [ProtoMember(2)]
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }


    }
}