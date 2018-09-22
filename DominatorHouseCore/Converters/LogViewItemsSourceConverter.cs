using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class LogViewItemsSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = values[0] as IEnumerable<LoggerModel>;
            if (collection == null)
            {
                return values[0];
            }

            var network = (SocialNetworks?)values[1];
            var activityType = (ActivityType?)values[2];
            var logLevel = values[3];
            collection = collection.Where(a => a.LogType.ToUpper() == logLevel.ToString());
            if (network.HasValue)
            {
                collection = collection.Where(a => a.Network.ToString() == network.ToString());
            }

            if (activityType.HasValue)
            {
                collection = collection.Where(a => a.ActivityType?.ToString() == activityType.ToString());
            }

            return collection.ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
