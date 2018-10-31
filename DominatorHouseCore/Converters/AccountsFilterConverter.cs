using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class AccountsFilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return null;
            var collection = values[0] as IEnumerable<DominatorAccountModel>;
            var socialNetworks = values[1] as SocialNetworks?;

            if (collection != null)
            {
                if (socialNetworks.HasValue && socialNetworks.Value != SocialNetworks.Social)
                {
                    collection = collection.Where(a => a.AccountBaseModel.AccountNetwork == socialNetworks.Value);
                }
            }

            return collection;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
