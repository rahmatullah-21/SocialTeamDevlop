using System;

namespace DominatorHouseCore.Utility
{
    public interface IDateProvider
    {
        DateTime UtcNow();
        DateTime Now();
    }

    public sealed class DateProvider : IDateProvider
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }

        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
