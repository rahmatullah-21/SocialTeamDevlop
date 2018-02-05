using System;
using System.Net;

namespace DominatorHouseCore.Requests
{
    public static class WebHelper
    {
        public static string ConvertValidHttpsUrl(this string value)
        {
            value = value.Replace(" ", string.Empty);
            if (value.StartsWith("https://"))
                return value;
            value = value.StartsWith("http://") ? value.Replace("http://", "https://") : value.Insert(0, "https://");
            return value;
        }

        public static WebHelper.WebExceptionIssue GetErrorMsgWebrequest(this WebException ex)
        {
            switch (ex.Status)
            {
                case WebExceptionStatus.Success:
                    throw new NotImplementedException();
                case WebExceptionStatus.NameResolutionFailure:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "No connection",
                        MessageLong = "Unable to resolve host name.",
                        MessageSolution = "Make sure there is an internet connection. Make sure your DNS server is working."
                    };
                case WebExceptionStatus.ConnectFailure:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "No connection",
                        MessageLong = "Unable to make a request to the host. Unable to create a connection.",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.ReceiveFailure:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "Server Error",
                        MessageLong = "A complete response was not received from the remote server.",
                        MessageSolution = "Make sure there is an internet connection and that your connection is stable."
                    };
                case WebExceptionStatus.SendFailure:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "DNS Error",
                        MessageLong = "A complete request could not be sent to the remote server.",
                        MessageSolution = "Make sure there is an internet connection and that your connection is stable."
                    };
                case WebExceptionStatus.PipelineFailure:
                    throw new NotImplementedException();
                case WebExceptionStatus.RequestCanceled:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "Request canceled",
                        MessageLong = "The request was canceled/aborted.",
                        MessageSolution = "Contact support"
                    };
                case WebExceptionStatus.ProtocolError:
                    switch (((HttpWebResponse)ex.Response).StatusCode)
                    {
                        case HttpStatusCode.RequestTimeout:
                            return new WebHelper.WebExceptionIssue()
                            {
                                MessageShort = "Bad Request",
                                MessageLong = "The data you sent was not expected by the server.",
                                MessageSolution = "Make sure the data you send to the server is correct. If the issue persists, contact support."
                            };
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.BadGateway:
                            return new WebHelper.WebExceptionIssue()
                            {
                                MessageShort = "Connection error",
                                MessageLong = "An incomplete request was made.",
                                MessageSolution = "Make sure there is an internet connection. If the issue persists, contact support."
                            };
                        case HttpStatusCode.ServiceUnavailable:
                            return new WebHelper.WebExceptionIssue()
                            {
                                MessageShort = "Connection error",
                                MessageLong = "Server has closed.",
                                MessageSolution = "Make sure there is an internet connection. If the issue persists, contact support."
                            };
                        default:
                            throw new NotImplementedException();
                    }
                case WebExceptionStatus.ConnectionClosed:
                    throw new NotImplementedException();
                case WebExceptionStatus.TrustFailure:
                    throw new NotImplementedException();
                case WebExceptionStatus.SecureChannelFailure:
                    throw new NotImplementedException();
                case WebExceptionStatus.ServerProtocolViolation:
                    throw new NotImplementedException();
                case WebExceptionStatus.KeepAliveFailure:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "Connection error",
                        MessageLong = "The connection for a request that specifies the Keep-alive header was closed unexpectedly.",
                        MessageSolution = "Make sure there is an internet connection. If the issue persists, contact support."
                    };
                case WebExceptionStatus.Pending:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "Connection error",
                        MessageLong = "An internal asynchronous request is pending.",
                        MessageSolution = "Contact support"
                    };
                case WebExceptionStatus.Timeout:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "Request Timed Out",
                        MessageLong = "When requesting an url, the request timed out.",
                        MessageSolution = "Make sure there is an internet connection, or try to increase the request timeout."
                    };
                case WebExceptionStatus.ProxyNameResolutionFailure:
                    return new WebHelper.WebExceptionIssue()
                    {
                        MessageShort = "No connection",
                        MessageLong = "The name resolver service could not resolve the proxy host name.",
                        MessageSolution = "Make sure there is an internet connection."
                    };
                case WebExceptionStatus.UnknownError:
                    throw new NotImplementedException();
                case WebExceptionStatus.MessageLengthLimitExceeded:
                    throw new NotImplementedException();
                case WebExceptionStatus.CacheEntryNotFound:
                    throw new NotImplementedException();
                case WebExceptionStatus.RequestProhibitedByCachePolicy:
                    throw new NotImplementedException();
                case WebExceptionStatus.RequestProhibitedByProxy:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool IsValidUriString(this string uri)
        {
            Uri result;
            if (uri == "N/A" || !Uri.TryCreate(uri, UriKind.Absolute, out result))
                return false;
            if (!(result.Scheme == Uri.UriSchemeHttp))
                return result.Scheme == Uri.UriSchemeHttps;
            return true;
        }

        public class WebExceptionIssue
        {
            public string MessageLong { get; set; }

            public string MessageShort { get; set; }

            public string MessageSolution { get; set; }
        }
    }
}
