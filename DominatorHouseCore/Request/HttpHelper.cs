using DominatorHouseCore.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Request
{
    public class HttpHelper : IHttpHelper
    {
        static HttpHelper()
        {
            ValidateServerCertificate();

        }
        //public HttpHelper() : this(new RequestParameters())
        //{
        //    // call to valid the certificates
        //    Request = _request;
        //}

        //public HttpHelper(IRequestParameters requestParameters)
        //{
        //    // set the web header details      
        //    SetRequestParameter(requestParameters);
        //    Request  = _request;
        //}


        ///// <summary>
        ///// Gets or Sets the header details of <see cref="HttpHelper"/>
        ///// </summary>
        //protected virtual IRequestParameters RequestParameters { get; set; } = new RequestParameters();

        //public HttpWebRequest Request
        //{
        //    get { return _request; }
        //    set { _request = value; }
        //}

        //protected HttpWebRequest _request;
       

        //public HttpWebResponse Response { get; set; }

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
                ex.DebugLog();
            }
        }

        ///// <summary>
        ///// Returns <see cref="IRequestParameters"/> which contains header details of 
        ///// <see cref="HttpHelper"/>
        ///// </summary>
        ///// <returns>A <see cref="IRequestParameters"/></returns>
        //public virtual IRequestParameters GetRequestParameter()
        //    => RequestParameters;

        ///// <summary>
        ///// Set the web header details to <see cref="RequestParameters"/>
        ///// </summary>
        ///// <param name="requestParameters">Pass the class which inherits <see cref="IRequestParameters"/> interface</param>
        //public virtual void SetRequestParameter(IRequestParameters requestParameters)
        //    => RequestParameters = requestParameters;



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
                            ex.DebugLog();
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

                    foreach (Cookie eachCookie in requestParameter.Cookies)
                    {
                        try
                        {
                            var cookieData = new Cookie(eachCookie.Name, eachCookie.Value, "/", eachCookie.Domain);
                            webRequest.CookieContainer.Add(cookieData);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
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
                ex.DebugLog();
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
                ex.DebugLog();
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
                var postBuffer = Encoding.UTF8.GetBytes(postData);
                WritePostData(ref webRequest, postBuffer);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
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
                ex.DebugLog();
            }
        }


        /// <summary>
        /// Read the cookies from <see cref="System.Net.WebResponse"/> object to RequestParameters
        /// </summary>
        /// <param name="webResponse"><see cref="System.Net.WebResponse"/></param>
        protected virtual void ReadCookies(HttpWebResponse webResponse,IRequestParameters requestParameter,HttpWebRequest request)
        {
            try
            {
                if (webResponse == null) return;

              //  var response = webResponse as HttpWebResponse;

                if (requestParameter.Cookies == null)
                {
                    requestParameter.Cookies = new CookieCollection();
                }

                if (webResponse == null) return;

                var cookies = request.CookieContainer.GetCookies(request.RequestUri);

                foreach (Cookie cookie in cookies)
                {
                    // check the current cookie is any already present in RequestParameter
                    var isPresent =
                        requestParameter.Cookies.Cast<Cookie>()
                        .Any(requestParameterCookie => requestParameterCookie.Name == cookie.Name);

                    // If its present read then overwrite otherwise add to RequestParameter
                    if (isPresent)
                    {
                        if (!string.IsNullOrEmpty(requestParameter.Cookies[cookie.Name]?.Value))
                            requestParameter.Cookies[cookie.Name].Value = cookie.Value;
                    }
                    else
                        requestParameter.Cookies.Add(cookie);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        /// <summary>
        /// Get the final response of the http request
        /// </summary>
        /// <returns><see cref="IResponseParameter"/></returns>
        protected virtual IResponseParameter GetFinalResponse(HttpWebRequest request, IRequestParameters requestParameters)
        {
            return DoGetFinalResponse(request, requestParameters);
        }

        private IResponseParameter DoGetFinalResponse(HttpWebRequest request,IRequestParameters requestParameters,Func<bool> wasCancelled = null)
        {
            try
            {
                // Get the reponse from request
                return GetReponse((HttpWebResponse)request.GetResponse(),requestParameters, request);
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
                        ? GetReponse((HttpWebResponse)ex.Response, requestParameters, request)
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
                return new ResponseParameter
                {
                    HasError = true,
                    Exception = ex
                };
            }
        }

        protected virtual async Task<IResponseParameter> GetFinalResponseAsync(CancellationToken cancellationToken, IRequestParameters requestParameters, HttpWebRequest request)
        {

            if (cancellationToken == default(CancellationToken))
            {
                return await DoGetFinalResponseAsync(request, requestParameters);
            }

            using (cancellationToken.Register(() => request.Abort()))
            {
                return await DoGetFinalResponseAsync(request, requestParameters,() => cancellationToken.IsCancellationRequested);
            }

        }

        private async Task<IResponseParameter> DoGetFinalResponseAsync(HttpWebRequest request, IRequestParameters requestParameters, Func<bool> wasCancelled = null)
        {
            try
            {
                // Get the reponse from request
                return GetReponse((HttpWebResponse)await request.GetResponseAsync(),requestParameters,request);
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
                        ? GetReponse((HttpWebResponse)ex.Response,requestParameters, request)
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
        protected virtual IResponseParameter GetReponse(HttpWebResponse webResponse,IRequestParameters requestParameters, HttpWebRequest request)
        {

            // pointing same address Response and webResponse
            //Response = webResponse;

            // Read the cookies from webresponse to RequestParameter
            ReadCookies(webResponse, requestParameters, request);

            // Get the streams from Response
            var responseStream = webResponse.GetResponseStream();

            // Check null integrity
            if (responseStream == null)
                return new ResponseParameter { Response = string.Empty };

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

        ///// <summary>
        ///// Get http request from url with already setted RequestParameters
        ///// </summary>
        ///// <param name="url">url of tha page</param>
        ///// <returns><see cref="IResponseParameter"/></returns>
        //public virtual IResponseParameter GetRequest(string url)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(url);
        //    SetRequestParametersToWebRequest(ref request, RequestParameters);
        //    return GetFinalResponse();
        //}

        /// <summary>
        /// Get http request from url with new RequestParameters
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="requestParameters"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter GetRequest(string url, IRequestParameters requestParameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref request, requestParameters);
            return GetFinalResponse(request,requestParameters);
        }


        ///// <summary>
        ///// Asynchronous Get Request
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //public virtual Task<IResponseParameter> GetRequestAsync(string url, CancellationToken cancellationToken, IRequestParameters requestParameters)
        //{
        //    var request = (HttpWebRequest) WebRequest.Create(url);
        //    try
        //    {
        //        SetRequestParametersToWebRequest(ref request, requestParameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    return GetFinalResponseAsync(cancellationToken, requestParameters, request);
        //}

        /// <summary>
        /// Async Get Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters, CancellationToken cancellationToken)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref request, requestParameters);
            return GetFinalResponseAsync(cancellationToken, requestParameters, request);
        }

        /// <summary>
        ///// Post Request with url and postdata with already saved RequestParameter
        ///// </summary>
        ///// <param name="url">url of tha page</param>
        ///// <param name="postData">post data in byte array which while pass with url</param>
        ///// <returns><see cref="IResponseParameter"/></returns>
        //public virtual IResponseParameter PostRequest(string url, byte[] postData)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(url);
        //    SetRequestParametersToWebRequest(ref request, RequestParameters);
        //    WritePostData(ref request, postData);
        //    return GetFinalResponse();
        //}

        public virtual Task<IResponseParameter> PostRequestAsync(string url, byte[] postData, CancellationToken cancellationToken, IRequestParameters requestParamater)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref request, requestParamater);
            WritePostData(ref request, postData);
            return GetFinalResponseAsync(cancellationToken, requestParamater,request);
        }

        /// <summary>
        /// Post Request with url and postdata as sequences of bytes with new RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <param name="requestParamater"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        // ReSharper disable once UnusedMemberHierarchy.Global
        public virtual IResponseParameter PostRequest(string url, byte[] postData, IRequestParameters requestParamater)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref request, requestParamater);
            WritePostData(ref request, postData);
            return GetFinalResponse(request,requestParamater);
        }

        ///// <summary>
        ///// Post Request with url and postdata with already saved RequestParameter
        ///// </summary>
        ///// <param name="url">url of tha page</param>
        ///// <param name="postData">post data which while pass with url</param>
        ///// <returns><see cref="IResponseParameter"/></returns>
        //public virtual IResponseParameter PostRequest(string url, string postData)
        //{
        //    _request = (HttpWebRequest)WebRequest.Create(url);
        //    SetRequestParametersToWebRequest(ref _request, RequestParameters);
        //    WritePostData(ref _request, postData);
        //    return GetFinalResponse();
        //}

        /// <summary>
        /// Post Request with url and postdata with new RequestParameter
        /// </summary>
        /// <param name="url">url of tha page</param>
        /// <param name="postData">post data in byte array which while pass with url</param>
        /// <param name="requestParamater"><see cref="IRequestParameters"/></param>
        /// <returns><see cref="IResponseParameter"/></returns>
        public virtual IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParamater)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Host = request.RequestUri.Host;
            SetRequestParametersToWebRequest(ref request, requestParamater);
            WritePostData(ref request, postData);
            return DoGetFinalResponse(request, requestParamater);
        }

        /// <summary>
        /// Async Post Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<IResponseParameter> PostRequestAsync(string url, string postData, CancellationToken cancellationToken, IRequestParameters requestParameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            SetRequestParametersToWebRequest(ref request, requestParameters);
            WritePostData(ref request, postData);
            return GetFinalResponseAsync(cancellationToken,requestParameters,request);
        }

        ///// <summary>
        ///// Async Post Request With RequestParameters
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="postData"></param>
        ///// <param name="requestParameters"></param>
        ///// <returns></returns>
        //public virtual Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParameters)
        //{

        //    _request = (HttpWebRequest)WebRequest.Create(url);
        //    Response = null;
        //    SetRequestParametersToWebRequest(ref _request, requestParameters);
        //    WritePostData(ref _request, postData);
        //    var response = GetFinalResponseAsync(default(CancellationToken));
        //    return response;
        //}

        /// <summary>
        /// Async Post Request With RequestParameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="requestParamater"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParamater, CancellationToken cancellationToken)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Host = request.RequestUri.Host;
            SetRequestParametersToWebRequest(ref request, requestParamater);
            WritePostData(ref request, postData);
            return GetFinalResponseAsync(cancellationToken,requestParamater,request);
        }

        #endregion


        public static async Task<Stream> GetResponseStreamAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            var licenseresponse = (HttpWebResponse)await request.GetResponseAsync();

            var responseStream = licenseresponse.GetResponseStream();
            return responseStream;
        }

        public virtual IResponseParameter GetApiRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var responseStream = response.GetResponseStream();

                // Check null integrity
                if (responseStream == null)
                    return new ResponseParameter { Response = string.Empty };

                // return as proper ResponseParameter with appropriate reponse
                using (var streamReader = new StreamReader(responseStream))
                {
                    return new ResponseParameter()
                    {
                        Response = streamReader.ReadToEnd()
                    };
                };
            }
        }

        public virtual IResponseParameter PostApiRequest(string url, byte[] postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Host = "api.socinator.com";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";

            request.ContentLength = postData.Length;

            using (var postDataStream = request.GetRequestStream())
            {
                postDataStream.Write(postData, 0, postData.Length);
            }



            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var responseStream = response.GetResponseStream();

                // Check null integrity
                if (responseStream == null)
                    return new ResponseParameter { Response = string.Empty };

                // return as proper ResponseParameter with appropriate reponse
                using (var streamReader = new StreamReader(responseStream))
                {
                    return new ResponseParameter()
                    {
                        Response = streamReader.ReadToEnd()
                    };
                };
            }
        }


        public virtual IResponseParameter PostApiRequest(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Host = "api.socinator.com";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            var postDataBytes = Encoding.UTF8.GetBytes(postData);

            request.ContentLength = postDataBytes.Length;

            using (var postDataStream = request.GetRequestStream())
            {
                postDataStream.Write(postDataBytes, 0, postDataBytes.Length);
            }


            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {

                var responseStream = response.GetResponseStream();

                // Check null integrity
                if (responseStream == null)
                    return new ResponseParameter { Response = string.Empty };

                // return as proper ResponseParameter with appropriate reponse
                using (var streamReader = new StreamReader(responseStream))
                {
                    return new ResponseParameter()
                    {
                        Response = streamReader.ReadToEnd()
                    };
                };
            }
        }
    }
}
