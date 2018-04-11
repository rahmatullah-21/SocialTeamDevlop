using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel
{
    public class DestinationDetailsCollection : BindableBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string MembersCount { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                    return;
                SetProperty(ref _isSelected, value);
            }
        }
    }
}