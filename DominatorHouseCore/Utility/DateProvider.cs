#region

using System;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface IDateProvider
    {
        DateTime UtcNow();
        DateTime Now();
        DateTime Today();
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

        public DateTime Today()
        {
            return DateTime.Today;
        }
    }
}