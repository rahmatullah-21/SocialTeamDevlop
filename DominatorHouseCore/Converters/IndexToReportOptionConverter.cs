using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class IndexToReportOptionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return null;
            //var list = new List<string>() { "Sexual content", "Violent or repulsive content", "Hateful or abusive content", "Harmful dangerous acts", "Child abuse", "Promotes terrorism", "Spam or misleading", "Infringes my rights", "Captions issue" };

            //var option = values[0] as IEnumerable<DominatorAccountModel>;

            //var  = values[1] as SocialNetworks?;
            return null;
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
