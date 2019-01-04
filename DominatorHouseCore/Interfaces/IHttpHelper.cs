using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    /// Interface for HttpHelper
    /// </summary>
    public interface IHttpHelper
    {
        IResponseParameter GetApiRequest(string url);
        IResponseParameter GetRequest(string url, IRequestParameters requestParameters);
        Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters, CancellationToken cancellationToken);
        IResponseParameter PostApiRequest(string url, string postData);
        IResponseParameter PostApiRequest(string url, byte[] postData);
        IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParamater);
        IResponseParameter PostRequest(string url, byte[] postData, IRequestParameters requestParamater);
        Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParamater, CancellationToken cancellationToken);
        Task<IResponseParameter> PostRequestAsync(string url, string postData, CancellationToken cancellationToken, IRequestParameters requestParameters);
        Task<IResponseParameter> PostRequestAsync(string url, byte[] postData, CancellationToken cancellationToken, IRequestParameters requestParamater);

        //Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters, CancellationToken cancellationToken);
        ////Task<IResponseParameter> PostRequestAsync(string url, string postData);
        ////Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParameters);
        //IResponseParameter GetRequest(string url);
        //IResponseParameter GetRequest(string url, IRequestParameters requestParameters,HttpWebRequest request);
        //IResponseParameter PostRequest(string url, string postData);
        //IResponseParameter PostRequest(string url, byte[] postData);
        //IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParameters);

        ////Task<IResponseParameter> PostRequestAsync(string url, string postData, CancellationToken cancellationToken);
        //Task<IResponseParameter> PostRequestAsync(string url, byte[] postData, CancellationToken cancellationToken);
        //IResponseParameter PostApiRequest(string url, byte[] postData);
        ////HttpWebRequest Request { get; }
        ////HttpWebResponse Response { get; }
    }
}