using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces.StartUp
{
    public interface IStartupJobConfiguration
    {
        JobConfiguration JobConfiguration { get; set; }
    }
}
