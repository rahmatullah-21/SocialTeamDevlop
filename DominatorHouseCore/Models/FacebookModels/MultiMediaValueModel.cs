#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models.FacebookModels
{
    public class MultiMediaValueModel : BindableBase
    {
        private string _id = Utilities.GetGuid();

        public string Id
        {
            get => _id;
            set
            {
                if (value == _id)
                    return;
                SetProperty(ref _id, value);
            }
        }

        private string _mediaPath = string.Empty;

        public string MediaPath
        {
            get => _mediaPath;
            set
            {
                if (value == _mediaPath)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }

        private double _mediaHeight = 150;

        public double MediaHeight
        {
            get => _mediaHeight;
            set
            {
                if (value == _mediaHeight)
                    return;
                SetProperty(ref _mediaHeight, value);
            }
        }

        private double _closeButtonHeight = 150;

        public double CloseButtonHeight
        {
            get => _closeButtonHeight;
            set
            {
                if (value == _closeButtonHeight)
                    return;
                SetProperty(ref _closeButtonHeight, value);
            }
        }
    }
}