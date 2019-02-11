using System.Net.Http;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public interface IWebService
    {
        Task<byte[]> GetImageBytesFromUrl(string url);
    }

    public sealed class WebService : IWebService
    {
        public async Task<byte[]> GetImageBytesFromUrl(string url)
        {
            return await new HttpClient().GetByteArrayAsync(url).ConfigureAwait(false);
        }
    }
}
