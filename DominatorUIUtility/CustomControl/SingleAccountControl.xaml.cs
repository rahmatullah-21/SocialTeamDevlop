using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for SingleAccountControl.xaml
    /// </summary>
    public partial class SingleAccountControl : CustomDialog
    {
        SingleAccountModel objSingleAccountModel = new SingleAccountModel();

        public SingleAccountControl()
        {
            InitializeComponent();

        }
        public SingleAccountControl(SingleAccountModel objSingleAccountModelBinding) : this()
        {
            this.objSingleAccountModel = objSingleAccountModelBinding;


            this.DataContext = objSingleAccountModel;
        }
    }

    public class SingleAccountModel : BindableBase
    {
        private string _pageTitle;

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                if (_pageTitle != null && value == _pageTitle)
                    return;
                SetProperty(ref _pageTitle, value);
            }
        }

        private string _groupName;

        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName != null && value == _groupName)
                    return;
                SetProperty(ref _groupName, value);
            }
        }


        private string _userName;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != null && value == _userName)
                    return;
                SetProperty(ref _userName, value);
            }
        }


        private Proxy _accountProxy = new Proxy();

        public Proxy AccountProxy
        {
            get
            {
                return _accountProxy;
            }
            set
            {
                if (_accountProxy != null && value == _accountProxy)
                    return;
                SetProperty(ref _accountProxy, value);
            }
        }

        private string _password;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {

                if (_password != null && value == _password)
                    return;
                SetProperty(ref _password, value);
            }
        }

        private Visibility? _advancedOptions = Visibility.Collapsed;

        public Visibility? AdvancedOptions
        {
            get
            {
                return _advancedOptions;
            }
            set
            {
                if (_advancedOptions != null && value == _advancedOptions)
                    return;
                SetProperty(ref _advancedOptions, value);

            }
        }

        private bool? _showAdvancedSettings = false;

        public bool? ShowAdvancedSettings
        {
            get
            {
                return _showAdvancedSettings;
            }
            set
            {
                if (_showAdvancedSettings != null && value == _showAdvancedSettings)
                    return;

                if (value == true)
                    AdvancedOptions = Visibility.Visible;
                else
                    AdvancedOptions = Visibility.Collapsed;

                SetProperty(ref _showAdvancedSettings, value);
            }
        }



        private string _btnContent;

        public string BtnContent
        {
            get
            {

                return _btnContent;
            }
            set
            {
                if (_btnContent != null && value == _btnContent)
                    return;
                SetProperty(ref _btnContent, value);
            }
        }

    }



}
