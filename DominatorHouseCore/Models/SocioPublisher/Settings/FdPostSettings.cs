using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class FdPostSettings : BindableBase, IFdPostSettings 
    {
        private bool _sellPostSelected;
        private string _productName = string.Empty;
        private int _productPrice;
        private string _productAvailableLocation = string.Empty;
        private bool _sellPostTurnOffComment;
        private bool _isReplaceDescriptionSelected;
        private string _postReplaceDescription = string.Empty;

        [ProtoMember(1)]
        public bool SellPostSelected
        {
            get
            {
                return _sellPostSelected;
            }
            set
            {
                if (_sellPostSelected == value)
                    return;
                _sellPostSelected = value;
                SetProperty(ref _sellPostSelected, value);
            }
        }

        [ProtoMember(2)]
        public string ProductName
        {
            get
            {
                return _productName;
            }
            set
            {
               
                if (_productName == value)
                    return;
                _productName = value;
                SetProperty(ref _productName, value);
            }
        }

        [ProtoMember(3)]
        public int ProductPrice
        {
            get
            {
                return _productPrice;
            }
            set
            {               
                _productPrice = value;
                SetProperty(ref _productPrice, value);
            }
        }

        [ProtoMember(4)]
        public string ProductAvailableLocation
        {
            get
            {
                return _productAvailableLocation;
            }
            set
            {             
                if (_productAvailableLocation == value)
                    return;
                _productAvailableLocation = value;
                SetProperty(ref _productAvailableLocation, value);
            }
        }

        [ProtoMember(5)]
        public bool SellPostTurnOffComment
        {
            get
            {
                return _sellPostTurnOffComment;
            }
            set
            {              
                if (_sellPostTurnOffComment == value)
                    return;
                _sellPostTurnOffComment = value;
                SetProperty(ref _sellPostTurnOffComment, value);
            }
        }

        [ProtoMember(6)]
        public bool IsReplaceDescriptionSelected
        {
            get
            {
                return _isReplaceDescriptionSelected;
            }
            set
            {              
                if (_isReplaceDescriptionSelected == value)
                    return;
                _isReplaceDescriptionSelected = value;
                SetProperty(ref _isReplaceDescriptionSelected, value);
            }
        }

        [ProtoMember(7)]
        public string PostReplaceDescription
        {
            get
            {
                return _postReplaceDescription;
            }
            set
            {
                if (_postReplaceDescription == value)
                    return;
                _postReplaceDescription = value;
                SetProperty(ref _postReplaceDescription, value);
            }
        }

       
    }
}