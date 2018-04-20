using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Requests;

namespace DominatorHouseCore.Request
{
    public class HttpHelper : IHttpHelper
    {

        public HttpHelper()
        {
            // call to valid the certificates
            ValidateServerCertificate();
        }

        public HttpHelper(IRequestParameters requestParameters) : this()
        {
            // set the web header details      
            SetRequestParameter(requestParameters);
        }

        /// <summary>
        /// Gets or Sets the header details of <see cref="HttpHelper"/>
        /// </summary>
        protected virtual IRequestParameters RequestParameters { get; set; } = new RequestParameters();

        protected HttpWebRequest Request = null;

        protected HttpWebResponse Response = null;

        /// <summary>
        /// Validate the server certificates
        /// </summary>
        private static void ValidateServerCertificate()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback
                    = delegate { return true; };
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// Returns <see cref="IRequestParameters"/> which contains header details of 
        /// <see cref="HttpHelper"/>
        /// </summary>
        /// <returns>A <see cref="IRequestParameters"/></returns>
        public virtual IRequestParameters GetRequestParameter()
            => RequestParameters;

        /// <summary>
        /// Set the web header details to <see cref="RequestParameters"/>
        /// </summary>
        /// <param name="requestParameters">Pass the class which inherits <see cref="IRequestParameters"/> interface</param>
        public virtual void SetRequestParameter(IRequestParameters requestParameters)
            => RequestParameters = requestParameters;


      
        /// <summary>
        /// <para>Set <see cref="IRequestParameters"/> details to the followings.</para>
        /// <para> 1.<see cref="IRequestParameters.Headers"/> to <see cref="HttpWebRequest.Headers"/></para>
        /// <para> 2.<see cref="IRequestParameters.Cookies"/> to <see cref="HttpWebRequest.CookieContainer"/></para>
        /// <para> 3.<see cref="IRequestParameters.Proxy"/> to <see cref="HttpWebRequest.Proxy"/></para>
        /// </summary>
        /// <param name="webRequest"><see cref="HttpWebRequest"/></param>
        /// <param name="requestParameter"><see cref="IRequestParameters"/></param>
        protected virtual void SetRequestParametersToWebRequest(ref HttpWebRequest webRequest, IRequestParameters requestParameter)
        {
            try
            {
                if (requestParameter == null)
                    return;

                #region Set the Headers

                webRequest.Headers = new WebHeaderCollection();

                if (requestParameter.Headers != null)
                {
                    foreach (var eachHeader in requestParameter.Headers)
                    {
                        try
                        {
                            var headerName = eachHeader.ToString();

                            var headerValue = requestParameter.Headers[headerName];

                            if (headerName == "X-CSRFToken")
                            {
                                var token = requestParameter.Cookies["csrftoken"]?.Value;
                                webRequest.Headers.Add(eachHeader.ToString(), token);
                            }
                            else
                            {
                                if (!WebHeaderCollection.IsRestricted(headerName))
                                    webRequest.Headers.Add(headerName, headerValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.ErrorLog();
                        }
                    }
                }
                webRequest.Host = webRequest.RequestUri.Host;
                webRequest.KeepAlive = requestParameter.KeepAlive;
                webRequest.UserAgent = requestParameter.UserAgent;
                webRequest.ContentType = requestParameter.ContentType;
                webRequest.Referer = requestParameter.Referer;
                webRequest.Accept = requestParameter.Accept;

                #endregion

                if (ServicePointManager.Expect100Continue)
                {
                    ServicePointManager.Expect100Continue = false;
                }

                #region Set the Cookies

                if (requestParameter.Cookies != null)
                {
                    webRequest.CookieContainer = new CookieContainer();

                    foreach (Cookie eachCookie in RequestParameters.Cookies)
                    {
                        try
                        {
                            var cookieData = new Cookie(eachCookie.Name, eachCookie.Value, "/", eachCookie.Domain);
                            webRequest.CookieContainer.Add(cookieData);
                        }
                        catch (Exception ex)
                        {
                            ex.ErrorLog();
                        }
                    }
                }

                #endregion

                #region Set the Proxy

                if (!string.IsNullOrEmpty(requestParameter.Proxy?.ProxyIp))
                    SetProxy(ref webRequest, requestParameter);

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        /// <summary>
        /// Set the <see cref="IRequestParameters.Proxy"/> to <see cref="HttpWebRequest.Proxy"/>
        /// </summary>
        /// <param name="webRequest"><see cref="HttpWebRequest"/></param>
        /// <param name="requestParameter"><see cref="IRequestParameters"/></param>
        protected void SetProxy(ref HttpWebRequest webRequest, IRequestParameters requestParameter)
        {
            try
            {
                var webProxy = new WebProxy(requestParameter.Proxy.ProxyIp, int.Parse(requestParameter.Proxy.ProxyPort))
                {
                    BypassProxyOnLocal = true
                };

                if (!string.IsNullOrEmpty(requestParameter.Proxy.ProxyUsername)
                    && !string.IsNullOrEmpty(requestParameter.Proxy.ProxyPassword))
                {
                    webProxy.Credentials = new NetworkCredential(requestParameter.Proxy.ProxyUsername, requestParameter.Proxy.ProxyPassword);
                }
                webRequest.Proxy = webProxy;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        /// <summary>
        /// Convert the postdata(string) to sequences of bytes and write into the <see cref="HttpWebRequest"/> 
        /// </summary>
        /// <param name="webRequest"><see cref="HttpWebRequest"/></param>
        /// <param name="postData">Data which should pass as post data</param>
        protected virtual void WritePostData(ref HttpWebRequest webRequest, string postData)
        {
            try
            {
                var postBuffer = System.Text.Encoding.UTF8.GetBytes(postData);
                WritePostData(ref webRequest, postBuffer);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        /// <summary>
        /// Convert the postdata(sequences of bytes) to streams
        /// </summary>
        /// <param name="webRequest"><see cref="HttpWebRequest"/></param>
        /// <param name="postBuffer">Post data in bytes array</param>
        protected virtual void WritePostData(ref HttpWebRequest webRequest, byte[] postBuffer)
        {
            try
            {
                webRequest.Method = "POST";
                webRequest.ContentLength = postBuffer.Length;
                using (var postDataStream = webRequest.GetRequestStream())
                {
                    postDataStream.Write(postBuffer, 0, postBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        /// <summary>
        /// Read the cookies from <see cref="System.Net.WebResponse"/> object to RequestParameters
        /// </summary>
        /// <param name="webResponse"><see cref="System.Net.WebResponse"/></param>
        protected virtual void ReadCookies(WebResponse webResponse)
        {
            try
            {
                if (webResponse == null) return;

                var response = webResponse as HttpWebResponse;

                if (RequestParameters.Cookies == null)
                {
                    RequestParameters.Cookies = new CookieCollection();
                }

                if (response == null) return;

                var cookies = Request.CookieContainer.GetCookies(Request.RequestUri);

                foreach (Cookie cookie in cookies)
                {
                    // check the current cookie is any already present in RequestParameter
                    var isPresent =
                        RequestParameters.Cookies.Cast<Cookie>()
                        .Any(requestParameterCookie => requestParameterCookie.Name == cookie.Name);

                    // If its present read then overwrite otherwise add to RequestParameter
                    if (isPresent)
                    {
                        if (!string.IsNullOrEmpty(RequestParameters.Cookies[cookie.Name]?.Value))
                            RequestParameters.Cookies[cookie.Name].Value = cookie.Value;
                    }
                    else
                        RequestParameters.Cookies.Add(cookie);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        /// <summary>
        /// Get the final response of the http request
        /// </summary>
        /// <returns><see cref="IResponseParameter"/></returns>
        protected virtual IResponseParameter GetFinalResponse()
        {
            return GetFinalResponseAsync(default(CancellationToken)).Result;
        }

        protected virtual async Task<IResponseParameter> GetFinalResponseAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken == default(CancellationToken))
            {
                return await DoGetFinalResponseAsync();
            }
            else
            {
                using (cancellationToken.Register(() => Request.Abort()))
                {
                    return await DoGetFinalResponseAsync(() => cancellationToken.IsCancellationRequested);
                }
            }
        }

        private async Task<IResponseParameter> DoGetFinalResponseAsync(Func<bool> wasCancelled = null)
        {
            try
            {
                // Get the reponse from request
                return GetReponse((HttpWebResponse)await Request.GetResponseAsync());
            }
            catch (WebException ex)
            {
                if (wasCancelled != null && wasCancelled())
                {
                    throw new OperationCanceledException();
                }
                try
                {
                    // Get error message from the response
                    return ex.Response != null
                        ? GetReponse((HttpWebResponse)ex.Response)
                        : new ResponseParameter
                        {
                            HasError = true,
                            Exception = ex
                        };
                }
                catch (WebException exception)
                {
                    // return the exceptions of response in ResponseParameter
                    return new ResponseParameter()
                    {
                        HasError = true,
                        Exception = exception
                    };
                }
            }
            catch (Exception ex)
            {
                // return the actual exceptions in ResponseParameter
                return new ResponseParameter()
                {
                    HasError = true,
                    Exception = ex
                };
            }
        }


        /// <summary>
        /// Get the final response data from <see cref="System.Net.HttpWebResponse"/> objects to <see cref="IResponseParameter"/>
        /// </summary>
        /// <returns></returns>
        protected virtual IResponseParameter GetReponse(HttpWebResponse webResponse)
        {

            // pointing same address Response and webResponse
            Response = webResponse;

            // Read the cookies from webresponse to RequestParameter
            ReadCookies(Response);

            // Get the streams from Response
            var responseStream = Response.GetResponseStream();

            // Check null integrity
            if (responseStream == null)
                return new ResponseParameter {Response = string.Empty};

            // return as proper ResponseParameter with appropriate reponse
            using (var streamReader = new StreamReader(responseStream))
            {
                return new ResponseParameter()
                {
                    Response = streamReader.ReadToEnd()
                };
            }           
        }




        #region Get Request And Post Request With Synchronous And Asynchronous Methods

        /// <summary>
        /// Get http request from url with already setted RequestParameters
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter GetRequest(string url)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref Request, RequestParameters);
            return GetFinalResponse();
        }

        /// <summary>
        /// Get http request from url with new RequestParameters
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="requestParameters"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter GetRequest(string url, IRequestParameters requestParameters)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref Request, requestParameters);
            return GetFinalResponse();
        }


        /// <summary>
        /// Asynchronous Get Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual Task<IResponseParameter> GetRequestAsync(string url, CancellationToken cancellationToken)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref Request, this.RequestParameters);
            return GetFinalResponseAsync(cancellationToken);
        }



        /// <summary>
        /// Async Get Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParameters"></param>
        /// <returns></returns>
        public virtual Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters, CancellationToken cancellationToken)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref Request, requestParameters);
            return GetFinalResponseAsync(cancellationToken);
        }


        /// <summary>
        /// Post Request with url and postdata with already saved RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data which while pass with url</param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter PostRequest(string url, string postData)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref Request, this.RequestParameters);
            WritePostData(ref Request, postData);
            return GetFinalResponse();
        }


        /// <summary>
        /// Post Request with url and postdata with already saved RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter PostRequest(string url, byte[] postData)
        {
            return PostRequestAsync(url, postData, default(CancellationToken)).Result;
        }
        public virtual Task<IResponseParameter> PostRequestAsync(string url, byte[] postData, CancellationToken cancellationToken)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref Request, RequestParameters);
            WritePostData(ref Request, postData);
            return GetFinalResponseAsync(cancellationToken);
        }

        /// <summary>
        /// Post Request with url and postdata with new RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <param name="requestParamater"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParamater)
        {
            return PostRequestAsync(url, postData, requestParamater, default(CancellationToken)).Result;
        }

        public virtual Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParamater, CancellationToken cancellationToken)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            Request.Host = Request.RequestUri.Host;
            SetRequestParametersToWebRequest(ref Request, requestParamater);
            WritePostData(ref Request, postData);
            return GetFinalResponseAsync(cancellationToken);
        }


        /// <summary>
        /// Post Request with url and postdata as sequences of bytes with new RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <param name="requestParamater"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter PostRequest(string url, byte[] postData, IRequestParameters requestParamater)
        {
            Request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref Request, requestParamater);
            WritePostData(ref Request, postData);
            return GetFinalResponse();
        }


        /// <summary>
        /// Async Post Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public virtual Task<IResponseParameter> PostRequestAsync(string url, string postData, CancellationToken cancellationToken)
        {
            this.Request = (HttpWebRequest)WebRequest.Create(url);
            this.Response = null;
            SetRequestParametersToWebRequest(ref Request, RequestParameters);
            WritePostData(ref Request, postData);
            return GetFinalResponseAsync(cancellationToken);
        }


        /// <summary>
        /// Async Post Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="RequestParameters"></param>
        /// <returns></returns>
        //public virtual async Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters RequestParameters)
        //{
        //    throw new NotImplementedException();
        //    //Request = (HttpWebRequest)WebRequest.Create(url);
        //    //this.Response = null;
        //    //string Reponce = string.Empty;
        //    //SetRequestParametersToWebRequest(ref Request, this.RequestParameters);
        //    //WritePostData(ref Request, postData);
        //    //return GetFinalResponse();
        //}


        #endregion

    }
}
