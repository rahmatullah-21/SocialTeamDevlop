using DominatorHouseCore.LogHelper;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces;

namespace DominatorHouseCore.Requests
{
    public class HttpHelper : IHttpHelper
    {
        /// Initialize the httpHelper object , which will help to make httpwebrequest  
        #region cunstroctor
        public HttpHelper()
        {
            this.requestParameters = new RequestParameters();
            try
            {
                // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
            catch (Exception ex)
            { GlobusLogHelper.log.Error(ex.StackTrace); }

        }
        public HttpHelper(IRequestParameters requestParameters)
        {
            this.requestParameters = new RequestParameters();
            SetRequestParameter(requestParameters);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace); 
            }
        }
        #endregion

        protected virtual IRequestParameters requestParameters { get; set; }
        protected HttpWebRequest request = null;
        protected HttpWebResponse response = null;

        #region get and set request headers
        /// Get And Set Request Haeader 
        /// RequestHeaders contains Cookies , WebHeaders and Proxies , 
        /// Here we can get and set all these headers       

        public virtual IRequestParameters GetRequestParamaeter()
        {
            return this.requestParameters;
        }

        /// <summary>s
        /// set request header
        /// </summary>
        /// <param name="requestParameters"></param>
        public virtual void SetRequestParameter(IRequestParameters requestParameters)
        {
            this.requestParameters = requestParameters;
        }
        #endregion


        #region PreRequest And PostRequest Logics 
        /// Before Making Any Request -
        /// We Set Cookies , Proxy And WebHeaders
        /// After Request , We Store Cookies RequestHeader Cookie Field
        /// 
        //Set Cookies , Proxy And WebHeaders
        protected virtual void setRequestParametersToWebRequest(ref HttpWebRequest webRequest, IRequestParameters requestParamater)
        {
            try
            {
                if (requestParamater != null)
                {
                    if (requestParamater.Headers != null)
                    {
                        webRequest.Headers = new WebHeaderCollection();
                        if (requestParamater.Headers != null)
                        {
                            foreach (var eachHeader in requestParamater.Headers)
                            {
                                try
                                {
                                    string headerName = eachHeader.ToString();
                                    string headerValue = requestParamater.Headers[eachHeader.ToString()];

                                    if (headerName == "X-CSRFToken")
                                    {
                                        string token = requestParamater.Cookies["csrftoken"].Value;
                                        webRequest.Headers.Add(eachHeader.ToString(), token);
                                    }
                                    else
                                    {

                                        if (!WebHeaderCollection.IsRestricted(headerName))
                                        {
                                            webRequest.Headers.Add(headerName, headerValue);
                                        }
                                        // 
                                        //  webRequest.Headers[headerName] = headerValue;
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    GlobusLogHelper.log.Error(Ex.StackTrace + Ex.Message);
                                }
                            }
                        }
                    }
                    webRequest.Host = webRequest.RequestUri.Host;
                    webRequest.KeepAlive = requestParamater.KeepAlive;
                    webRequest.UserAgent = requestParamater.UserAgent;
                    webRequest.ContentType = requestParamater.ContentType;
                    webRequest.Referer = requestParamater.Referer;
                    webRequest.Accept = requestParamater.Accept;
                    webRequest.CookieContainer = new CookieContainer();
                    if (ServicePointManager.Expect100Continue == true)
                    {
                        ServicePointManager.Expect100Continue = false;
                    }

                    if (requestParamater.Cookies != null)
                    {
                        webRequest.CookieContainer = new CookieContainer();
                        foreach (System.Net.Cookie eachCookie in requestParameters.Cookies)
                        {
                            try
                            {
                                Cookie cookieData = new Cookie(eachCookie.Name, eachCookie.Value, "/", eachCookie.Domain);
                                webRequest.CookieContainer.Add(cookieData);
                            }
                            catch (Exception Ex)
                            { GlobusLogHelper.log.Error(Ex.StackTrace + Ex.Message); }
                        }


                    }
                    if (requestParamater.Proxy != null && !string.IsNullOrEmpty(requestParamater.Proxy.ProxyIp))
                    {
                        SetProxy(ref webRequest, requestParamater);

                    }
                }
            }
            catch (Exception Ex)
            {
                GlobusLogHelper.log.Error(Ex.StackTrace + Ex.Message);
            }
        }
        /// <summary>
        /// Set Proxy to each request
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="requestParamater"></param>
        private void SetProxy(ref HttpWebRequest webRequest, IRequestParameters requestParamater)
        {
            try
            {
                StringBuilder proxyAddress = new StringBuilder();
                proxyAddress.Append(requestParamater.Proxy.ProxyIp);
                int proxyPort = int.Parse(requestParamater.Proxy.ProxyPort);

                WebProxy myproxy = new WebProxy(proxyAddress.ToString(), proxyPort);
                myproxy.BypassProxyOnLocal = true;

                if (!string.IsNullOrEmpty(requestParamater.Proxy.ProxyUsername) && !string.IsNullOrEmpty(requestParamater.Proxy.ProxyPassword))
                {
                    StringBuilder proxyUsername = new StringBuilder();
                    proxyUsername.Append(requestParamater.Proxy.ProxyUsername);
                    StringBuilder proxyPassword = new StringBuilder();
                    proxyPassword.Append(requestParamater.Proxy.ProxyPassword);

                    myproxy.Credentials = new NetworkCredential(proxyUsername.ToString(), proxyPassword.ToString());
                }
                webRequest.Proxy = myproxy;
            }
            catch (Exception Ex)
            {
                GlobusLogHelper.log.Error(Ex.StackTrace + Ex.Message);
            }
        }

        /// <summary>
        /// Writing Post Data For Post Method WebRequest
        /// </summary>
        /// <param name="gRequest"></param>
        /// <param name="postData"></param>
        protected virtual void writePostData(ref HttpWebRequest gRequest, string postData)
        {
            try
            {
                gRequest.Method = "POST";
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.UTF8.GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                using (Stream postDataStream = gRequest.GetRequestStream())
                {
                    postDataStream.Write(postBuffer, 0, postBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        /// <summary>
        /// Writing Post Data For Post Method WebRequest
        /// </summary>
        /// <param name="gRequest"></param>
        /// <param name="postBuffer"></param>
        protected virtual void writePostData(ref HttpWebRequest gRequest, byte[] postBuffer)
        {
            try
            {
                gRequest.Method = "POST";
                gRequest.ContentLength = postBuffer.Length;
                using (Stream postDataStream = gRequest.GetRequestStream())
                {
                    postDataStream.Write(postBuffer, 0, postBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Store Cookies RequestHeader Cookie Field Taking From WebResponse 
        /// </summary>
        /// <param name="r"></param>
        protected virtual void ReadCookies(WebResponse r)
        {
            try
            {
                if (r != null)
                {
                    var response = r as HttpWebResponse;
                    if (this.requestParameters.Cookies == null)
                    {
                        this.requestParameters.Cookies = new CookieCollection();
                    }

                    if (response != null)
                    {
                     
                        CookieCollection cookies = request.CookieContainer.GetCookies(request.RequestUri);
                        foreach (Cookie cookie in cookies)
                        {
                            bool isPresent = false;

                            foreach (Cookie cookie_RequestParamete in this.requestParameters.Cookies)
                            {
                                if (cookie_RequestParamete.Name == cookie.Name)
                                {
                                    isPresent = true;
                                    break;
                                }
                            }
                            if (isPresent)
                            {
                                if (!string.IsNullOrEmpty(cookie.Value))
                                    this.requestParameters.Cookies[cookie.Name].Value = cookie.Value;
                            }
                            else
                            {
                                this.requestParameters.Cookies.Add(cookie);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                GlobusLogHelper.log.Error(Ex.StackTrace + Ex.Message);
            }

        }


        /// <summary>
        /// Get the final response
        /// </summary>
        /// <returns></returns>
        protected virtual IResponseParameter GetFinalResponse()
        {
            try
            {
                return GetReponce((HttpWebResponse) request.GetResponse());
            }
            catch (WebException Ex)
            {
                try
                {
                    return GetReponce((HttpWebResponse) Ex.Response);
                }
                catch (WebException Exn)
                {
                    return new ResponseParameter()
                    {
                        HasError = true,
                        Exception = Exn
                    };
                }
            }
            catch (Exception Ex)
            {
                return new ResponseParameter()
                {
                    HasError = true,
                    Exception = Ex
                };
            }
            finally
            {
            }

        }


        /// <summary>
        /// Get the final response and read it as string
        /// </summary>
        /// <returns></returns>
        protected virtual IResponseParameter GetReponce(HttpWebResponse webResponse)
        {
            #region read the response as a stream
            this.response = webResponse;
            ReadCookies(response);
            using (StreamReader streamReader = new StreamReader(this.response.GetResponseStream()))
            {
                return new ResponseParameter()
                {
                    Response = streamReader.ReadToEnd()
                };
            }
            #endregion
        }

        #endregion


        #region Get Request And Post Request With Synchronous And Asynchronous Methods

        /// <summary>
        /// We Create Request With HttpWebRequest
        /// We Create HttpWebRequest Instance , Set Cookies , WebHeaders And Proxies  
        /// Get The Request Result And Provide the Result In String Format .
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual IResponseParameter GetRequest(string url)
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
            setRequestParametersToWebRequest(ref request, requestParameters);
            return GetFinalResponse();
        }


        /// <summary>
        /// Get Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParameters"></param>
        /// <returns></returns>
        public virtual IResponseParameter GetRequest(string url, IRequestParameters requestParameters)
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
            setRequestParametersToWebRequest(ref request, requestParameters);
            return GetFinalResponse();
        }


        /// <summary>
        /// Asynchronous Get Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual async Task<IResponseParameter> GetRequestAsync(string url)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            setRequestParametersToWebRequest(ref request, this.requestParameters);
            await Task.Factory.StartNew(() =>
            {
                return GetFinalResponse();
            });
            return new ResponseParameter();
        }

       
        
        /// <summary>
        /// Async Get Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParameters"></param>
        /// <returns></returns>
        public virtual async Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            setRequestParametersToWebRequest(ref request, requestParameters);
            await Task.Factory.StartNew(() =>
            {
                return GetFinalResponse();
            });
            return new ResponseParameter();

        }


        /// <summary>
        /// Post Request With 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public virtual IResponseParameter PostRequest(string url, string postData)
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
            setRequestParametersToWebRequest(ref request, this.requestParameters);
            writePostData(ref request, postData);
            return GetFinalResponse();
        }


        /// <summary>
        /// Post requesst using bytes
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public virtual IResponseParameter postRequest(string url, byte[] postData)
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
            setRequestParametersToWebRequest(ref request, this.requestParameters);
            writePostData(ref request, postData);
            return GetFinalResponse();
        }

        /// <summary>
        /// Post Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="requestParamater"></param>
        /// <returns></returns>
        public virtual IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParamater)
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
            string Reponce = string.Empty;
            request.Host = request.RequestUri.Host;
            setRequestParametersToWebRequest(ref request, requestParamater);
            writePostData(ref request, postData);
            return GetFinalResponse();
        }


        /// <summary>
        /// Async Post Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public virtual async Task<IResponseParameter> PostRequestAsync(string url, string postData)
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
            string Reponce = string.Empty;
            this.response = null;
            setRequestParametersToWebRequest(ref request, requestParameters);
            writePostData(ref request, postData);
            return GetFinalResponse();
        }


        /// <summary>
        /// Async Post Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="RequestParameters"></param>
        /// <returns></returns>
        public virtual async Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters RequestParameters)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            this.response = null;
            string Reponce = string.Empty;
            setRequestParametersToWebRequest(ref request, requestParameters);
            writePostData(ref request, postData);
            return GetFinalResponse();
        }

       
        #endregion

    }
}
