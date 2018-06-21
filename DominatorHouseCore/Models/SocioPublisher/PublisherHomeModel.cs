using System.Windows.Controls;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class PublisherHomeModel : BindableBase
    {
        private UserControl _selectedUserControl;

        public UserControl SelectedUserControl
        {
            get
            {
                return _selectedUserControl;
            }
            set
            {
                if (_selectedUserControl != null && Equals(_selectedUserControl, value))
                    return;

                SetProperty(ref _selectedUserControl, value);
            }
        }
    }
}