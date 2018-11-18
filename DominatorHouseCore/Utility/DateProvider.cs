using System;

namespace DominatorHouseCore.Utility
{
    public interface IDateProvider
    {
        DateTime UtcNow();
    }

    public sealed class DateProvider : IDateProvider
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
