using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.Models.FacebookModels
{
    public class FbMultiMediaModel : BindableBase
    {
        private bool _isAddImageVisibile = true;
        public bool IsAddImageVisibile
        {
            get
            {
                return _isAddImageVisibile;
            }
            set
            {
                if (value == _isAddImageVisibile)
                    return;
                SetProperty(ref _isAddImageVisibile, value);
            }
        }

        private ObservableCollection<MultiMediaValueModel>
            _mediaPaths = new ObservableCollection<MultiMediaValueModel>();
        public ObservableCollection<MultiMediaValueModel> MediaPaths
        {
            get
            { return _mediaPaths; }
            set
            {
                if (value == _mediaPaths)
                    return;
                SetProperty(ref _mediaPaths, value);
            }
        }

        private bool _isMultiselect = true;
        public bool IsMultiselect
        {
            get
            {
                return _isMultiselect;
            }
            set
            {
                if (value == _isMultiselect)
                    return;
                SetProperty(ref _isMultiselect, value);
            }
        }

    }
}
