using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Requests;

namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    /// Interface for HttpHelper
    /// </summary>
    public interface IHttpHelper : IRequestHelper
    {
        Task<IResponseParameter> GetRequestAsync(string url, CancellationToken cancellationToken);
        Task<IResponseParameter> GetRequestAsync(string url, IRequestParameters requestParameters, CancellationToken cancellationToken);
        //Task<IResponseParameter> PostRequestAsync(string url, string postData);
        //Task<IResponseParameter> PostRequestAsync(string url, string postData, IRequestParameters requestParameters);
        IResponseParameter GetRequest(string url);
        IResponseParameter GetRequest(string url, IRequestParameters requestParameters);
        IResponseParameter PostRequest(string url, string postData);
        IResponseParameter PostRequest(string url, string postData, IRequestParameters requestParameters);
    }
}