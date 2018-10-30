using DominatorHouseCore.Models;
using System.Collections.Generic;

namespace DominatorHouseCore.ProxyServerManagment.Models
{
    public sealed class ProxyParsingResult
    {
        public IReadOnlyCollection<string> InvalidProxies { get; }
        public IReadOnlyCollection<ProxyManagerModel> Proxies { get; }
        public ProxyParsingResult(IReadOnlyCollection<string> invalidProxies, IReadOnlyCollection<ProxyManagerModel> proxies)
        {
            InvalidProxies = invalidProxies;
            Proxies = proxies;
        }
    }
}
