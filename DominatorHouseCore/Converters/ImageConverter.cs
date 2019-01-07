using CommonServiceLocator;
using DominatorHouseCore.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DominatorHouseCore.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            var constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();

            try
            {
                if (File.Exists(value?.ToString()) || ImageExtracter.IsValidUrl(value?.ToString()))
                    return string.IsNullOrEmpty(value?.ToString()) ? new BitmapImage() : new BitmapImage(new Uri(value.ToString()));

                if (!File.Exists(constantVariable.GetNotFoundImage()))
                {
                    Utilities.DownloadNotFound();
                }
                return new BitmapImage(new Uri(constantVariable.GetNotFoundImage()));

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new BitmapImage(new Uri(constantVariable.GetNotFoundImage()));
            }

        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

}