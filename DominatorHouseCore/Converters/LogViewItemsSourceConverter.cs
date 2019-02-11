using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class LogViewItemsSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var syncObject = values[0];
            lock (syncObject)
            {
                var collection = values[1] as IEnumerable<LoggerModel>;
                if (collection == null)
                {
                    return values[1];
                }

                IEnumerable<LoggerModel> copyCollection = collection.ToList();

                var network = (SocialNetworks?)values[2];
                var activityType = (ActivityType?)values[3];
                var logLevel = values[4];
                copyCollection = copyCollection.Where(a => a.LogType.ToUpper() == logLevel.ToString());
                if (network.HasValue)
                {
                    copyCollection = copyCollection.Where(a => a.Network.ToString() == network.ToString());
                }

                if (activityType.HasValue)
                {
                    copyCollection = copyCollection.Where(a => a.ActivityType?.ToString() == activityType.ToString());
                }

                return copyCollection.ToList();
            }
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
