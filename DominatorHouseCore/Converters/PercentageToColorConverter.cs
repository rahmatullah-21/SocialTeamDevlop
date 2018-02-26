using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(Brush))]
    public class PercentageToColorConverter : IValueConverter
    {

        public double GivenPercentage { get; set; } 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           

            if (value != null)
                GivenPercentage = double.Parse(value.ToString());

            if (GivenPercentage >= 25)

                if (GivenPercentage >= 25 && GivenPercentage < 50)
                    return Brushes.OrangeRed;

                else if (GivenPercentage >= 50 && GivenPercentage < 75)
                    return Brushes.CornflowerBlue;

                else
                    return Brushes.YellowGreen;

            else return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}