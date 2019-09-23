using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class MenuHandlerModel : BindableBase
    {
        private bool _isImportAccountsVisible;

        public bool IsImportMultipleAccountsVisible
        {
            get
            {
                return _isImportAccountsVisible;
            }
            set
            {
                if (_isImportAccountsVisible == value)
                    return;
                SetProperty(ref _isImportAccountsVisible, value);

            }
        }

        private bool _isSelctAccountsVisible;

        public bool IsSelectAccountsVisible
        {
            get
            {
                return _isSelctAccountsVisible;
            }
            set
            {
                if (_isSelctAccountsVisible == value)
                    return;
                SetProperty(ref _isSelctAccountsVisible, value);

            }
        }

        private bool _isUpdateAccountVisible;

        public bool IsUpdateAccountVisible
        {
            get
            {
                return _isUpdateAccountVisible;
            }
            set
            {
                if (_isUpdateAccountVisible == value)
                    return;
                SetProperty(ref _isUpdateAccountVisible, value);

            }
        }

        private bool _isExportAccountVisible;

        public bool IsExportAccountVisible
        {
            get
            {
                return _isExportAccountVisible;
            }
            set
            {
                if (_isExportAccountVisible == value)
                    return;
                SetProperty(ref _isExportAccountVisible, value);

            }
        }

        private bool _isDeleteAccountVisible;

        public bool IsDeleteAccountVisible
        {
            get
            {
                return _isDeleteAccountVisible;
            }
            set
            {
                if (_isDeleteAccountVisible == value)
                    return;
                SetProperty(ref _isDeleteAccountVisible, value);
            }
        }

        private bool _isBrowserAutomationVisible;

        public bool IsBrowserAutomationVisible
        {
            get
            {
                return _isBrowserAutomationVisible;
            }
            set
            {
                if (_isBrowserAutomationVisible == value)
                    return;
                SetProperty(ref _isBrowserAutomationVisible, value);

            }
        }

        private bool _isInfoVisible;

        public bool IsInfoVisible
        {
            get
            {
                return _isInfoVisible;
            }
            set
            {
                if (_isInfoVisible == value)
                    return;
                SetProperty(ref _isInfoVisible, value);

            }
        }

        private bool _isMenuHandlerVisible;

        public bool IsMenuHandlerVisible
        {
            get
            {
                return _isMenuHandlerVisible;
            }
            set
            {
                if (_isMenuHandlerVisible == value)
                    return;
                SetProperty(ref _isMenuHandlerVisible, value);

            }
        }

        private bool _isMenuHandlerChecked;

        public bool IsMenuHandlerChecked
        {
            get
            {
                return _isMenuHandlerChecked;
            }
            set
            {
                if (_isMenuHandlerChecked == value)
                    return;
                SetProperty(ref _isMenuHandlerChecked, value);
            }
        }

    }
}
