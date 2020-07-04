using DominatorHouseCore.LogHelper;
using ImageTypers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DominatorHouseCore.Utility.CaptchaSolverUtilities
{
    public class ImageTypersHelper
    {
        private readonly ImageTypersAPI _imgTypObj;
        private readonly string _proxy;
        public ImageTypersHelper(string accessToken, string proxy = null)
        {
            _imgTypObj = new ImageTypersAPI(accessToken);
            _proxy = proxy;
        }

        public bool IsAuthenticated()
        {
            try
            {
                MyBalance();
                return true;
            }
            catch (Exception ex)
            {
                var authFailed = ex.ToString().Contains("AUTHENTICATION_FAILED") ? " : please check your imagetypers access key" : "";
                GlobusLogHelper.log.Error($"Got Exception \"{ex}\" while checking balance{authFailed}");
                return false;
            }
        }

        public string MyBalance() => _imgTypObj.account_balance();


        public string SubmitSiteKey(string captchaUrl, string siteKey)
        {
            var d = new Dictionary<string, string> { { "page_url", captchaUrl }, { "sitekey", siteKey } };
            //if(!string.IsNullOrEmpty(_proxy.ToString()))
            // d.Add("proxy ", _proxy.ToString());

            return _imgTypObj.submit_recaptcha(d);
        }
        public string GetResponseImageCaptcha(string path)
        {
            string error = string.Empty;
            string captchaRes = string.Empty;
            do
            {
                try
                {
                    captchaRes = _imgTypObj.solve_captcha(path, true);
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                }
            } while (error.Contains("IMAGE_TIMED_OUT") || captchaRes.Contains("noanswer"));
            return captchaRes;
        }
        public string GetGResponseCaptcha(string captchaId, int delayInProgress = 7)
        {
            var isProgress = true;
            while (isProgress)
            {
                try
                {
                    isProgress = _imgTypObj.in_progress(captchaId);

                    if (isProgress)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(delayInProgress));
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.ToString().Contains("IMAGE_TIMED_OUT")) continue;
                    GlobusLogHelper.log.Info("Got Exception 'IMAGE_TIMED_OUT' while running function in_progress, Breaking the progress loop now");
                    break;
                }
            }

            if (isProgress)
            {
                GlobusLogHelper.log.Info("IMAGE_TIMED_OUT : Going to claim for Bad Captcha");
                SetBadCaptcha(captchaId);
                return "";
            }

            var gResponseToken = _imgTypObj.retrieve_captcha(captchaId);

            GlobusLogHelper.log.Info("Got g-Response token - " + gResponseToken);
            return gResponseToken;
        }

        public void SetBadCaptcha(string captchaId)
        {
            try
            {
                var setBadCaptchaResponse = _imgTypObj.set_captcha_bad(captchaId);

                GlobusLogHelper.log.Info(setBadCaptchaResponse.Contains("SUCCESS")
                    ? "Applied for Refund Successfully with ImageTypers"
                    : "Error => Got Wrong Response, Could Not Apply for Refund");


            }
            catch (Exception ex)
            {

            }
        }

    }
}
