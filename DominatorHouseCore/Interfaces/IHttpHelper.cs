using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    /// Interface for HttpHelper
    /// </summary>
    public interface IHttpHelper : IRequestHelper
    {
        Task<IResponseParameter> GetRequestAsync(string url, CancellationToken cancellationToken);
        Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters, CancellationToken cancellationToken);
        IResponseParameter GetRequest(string url);
        IResponseParameter GetRequest(string url, IRequestParameters requestParameters);
        IResponseParameter PostRequest(string url, string postData);
        IResponseParameter PostRequest(string url, byte[] postData);
        IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParameters);
        Task<IResponseParameter> PostRequestAsync(string url, string postData, CancellationToken cancellationToken);
        Task<IResponseParameter> PostRequestAsync(string url, byte[] postData, CancellationToken cancellationToken);
        IResponseParameter PostApiRequest(string url, byte[] postData);
        HttpWebRequest Request { get; }
        HttpWebResponse Response { get; }
        Task<IResponseParameter> PostApiRequestAsync(string url, string postData);
        Task<IResponseParameter> PostApiRequestAsync(string url, byte[] postData);
        Task<IResponseParameter> GetApiRequestAsync(string url);
    }
    public interface IHttpHelperAsync : IRequestHelper
    {
        HttpWebRequest Request { get; }
        HttpWebResponse Response { get; }
        Task WritePostDataAsync(HttpWebRequest webRequest, string postData);
        Task WritePostDataAsync(HttpWebRequest webRequest, byte[] postBuffer);
        Task<IResponseParameter> GetFinalResponseAsync();
        Task<IResponseParameter> DoGetFinalResponseAsync(Func<bool> wasCancelled = null);
        Task<IResponseParameter> GetFinalResponseAsync(CancellationToken cancellationToken);
        Task<IResponseParameter> GetReponseAsync(HttpWebResponse webResponse);
        Task<IResponseParameter> GetRequestAsync(string url, CancellationToken cancellationToken);
        Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters, CancellationToken cancellationToken);
        Task<IResponseParameter> PostRequestAsync(string url, byte[] postData);
        Task<IResponseParameter> PostRequestAsync(string url, byte[] postData, CancellationToken cancellationToken);
        Task<IResponseParameter> PostRequestAsync(string url, string postData, CancellationToken cancellationToken);
        Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParameters);
        Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParamater, CancellationToken cancellationToken);
        Task<IResponseParameter> GetApiRequest(string url);
        Task<IResponseParameter> PostApiRequest(string url, byte[] postData);
        Task<IResponseParameter> PostApiRequest(string url, string postData);
        Task<IResponseParameter> PostApiRequestAsync(string url, string postData);
        Task<IResponseParameter> GetApiRequestAsync(string url);
        Task<IResponseParameter> PostApiRequestAsync(string url, byte[] postData);
    }
}