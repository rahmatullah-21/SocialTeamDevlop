using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class EditProfileModel : BindableBase
    {

        private string _profilePicPath;
        private string _fullname;
        private string _username;
        private string _externalUrl;
        private string _bio;
        private string _email;
        private string _phoneNumber;
        private bool _isMaleChecked;
        private bool _isFemaleChecked;
        private bool _isNonSpecifiedChecked;
        [ProtoMember(1)]
        public string ProfilePicPath
        {
            get
            {
                return _profilePicPath;
            }
            set
            {
                _profilePicPath = value;
                OnPropertyChanged("ProfilePicPath");
            }
        }
        [ProtoMember(2)]
        public string Fullname
        {
            get
            {
                return _fullname;
            }
            set
            {
                _fullname = value;
                OnPropertyChanged("FullName");
            }
        }
        [ProtoMember(3)]
        public string Username
        {
            get
            {
                return _fullname;
            }
            set
            {
                _fullname = value;
                OnPropertyChanged("Username");
            }
        }
        [ProtoMember(4)]
        public string ExternalUrl
        {
            get
            {
                return _externalUrl;
            }
            set
            {
                _externalUrl = value;
                OnPropertyChanged("ExternalUrl");
            }
        }
        [ProtoMember(5)]
        public string Bio
        {
            get
            {
                return _bio;
            }
            set
            {
                _bio = value;
                OnPropertyChanged("Bio");
            }
        }
        [ProtoMember(6)]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }
        [ProtoMember(7)]
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }
        [ProtoMember(8)]
        public bool IsMaleChecked
        {
            get
            {
                return _isMaleChecked;
            }
            set
            {
                _isMaleChecked = value;
                OnPropertyChanged("IsMaleChecked");
            }
        }
        [ProtoMember(9)]
        public bool IsFemaleChecked
        {
            get
            {
                return _isFemaleChecked;
            }
            set
            {
                _isFemaleChecked = value;
                OnPropertyChanged("IsFemaleChecked");
            }
        }
        [ProtoMember(10)]
        public bool IsNonSpecifiedChecked
        {
            get
            {
                return _isNonSpecifiedChecked;
            }
            set
            {
                _isNonSpecifiedChecked = value;
                OnPropertyChanged("IsNonSpecifiedChecked");
            }
        }
    }
}