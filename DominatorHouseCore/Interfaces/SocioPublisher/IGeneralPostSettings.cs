using System;

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IGeneralPostSettings
    {

        DateTime? ExpireDate { get; set; }
        int ReaddCount { get; set; }
    }
}