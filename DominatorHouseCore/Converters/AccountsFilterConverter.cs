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
    public class AccountsFilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return null;

            var collection = values[0] as IEnumerable<DominatorAccountModel>;

            var socialNetworks = values[1] as SocialNetworks?;
            var doNotSortByUserName = values.Length == 4 ? (bool)values[3] : false;
            var isReturnwithoutAssign = parameter as bool?;
            if (collection != null)
            {
                if (socialNetworks.HasValue && socialNetworks.Value != SocialNetworks.Social)
                {
                    if (isReturnwithoutAssign == true)
                        return collection.Where(a => a.AccountBaseModel.AccountNetwork == socialNetworks.Value);
                    else
                        collection = collection.Where(a => a.AccountBaseModel.AccountNetwork == socialNetworks.Value);
                }

                if (!doNotSortByUserName)
                    collection = collection.OrderBy(x => x.AccountBaseModel.UserName).OrderBy(x => x.AccountBaseModel.AccountNetwork.ToString());

            }

            return collection;
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
