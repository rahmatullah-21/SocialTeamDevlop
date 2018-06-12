using System;

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IGeneralPostSettings
    {
        bool IsExpireDate { get; set; }

        DateTime ExpireDate { get; set; }
        bool IsReaddCount { get; set; }
        int ReaddCount { get; set; }
    }
}