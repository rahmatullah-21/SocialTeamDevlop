using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.Config
{
    public class UrlShortnerServicesViewModel : BindableBase
    {
        private UrlShortnerServicesModel _urlShortnerServicesModel = new UrlShortnerServicesModel();
        public UrlShortnerServicesModel UrlShortnerServicesModel
        {
            get
            {
                return _urlShortnerServicesModel;
            }
            set
            {
                if (_urlShortnerServicesModel == value)
                    return;
                SetProperty(ref _urlShortnerServicesModel, value);
            }
        }
    }
    public class CaptchaServicesViewModel : BindableBase
    {
        private CaptchaServicesModel _captchaServicesModel = new CaptchaServicesModel();
        public CaptchaServicesModel CaptchaServicesModel
        {
            get
            {
                return _captchaServicesModel;
            }
            set
            {
                if (_captchaServicesModel == value)
                    return;
                SetProperty(ref _captchaServicesModel, value);
            }
        }
    }
}
