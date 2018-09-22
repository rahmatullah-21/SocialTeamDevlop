using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class ActivityTypesSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = values[0] as IEnumerable<ActivityType?>;
            if (!(values[1] is SocialNetworks network) || network == SocialNetworks.Social)
            {
                return collection;
            }

            return collection.Where(a => a?.IsSupportedByNetwork(network) ?? false).ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
