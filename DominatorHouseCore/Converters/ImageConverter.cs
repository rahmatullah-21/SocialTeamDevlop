using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                #region Avoid Blocking Images

                //var imagePath = value?.ToString();
                //if (string.IsNullOrEmpty(imagePath))
                //    return new BitmapImage();
                //var bitmap = new BitmapImage();
                //var stream = File.OpenRead(imagePath);
                //bitmap.BeginInit();
                //bitmap.CacheOption = BitmapCacheOption.OnLoad;
                //bitmap.StreamSource = stream;
                //bitmap.EndInit();
                //stream.Close();
                //stream.Dispose();
                //return bitmap;

                #endregion

                if (File.Exists(value?.ToString()) || ImageExtracter.IsValidUrl(value?.ToString()))
                    return string.IsNullOrEmpty(value?.ToString()) ? new BitmapImage() : new BitmapImage(new Uri(value.ToString()));

                if (!File.Exists(ConstantVariable.GetNotFoundImage()))
                {
                    Utilities.DownloadNotFound();
                }
                return new BitmapImage(new Uri(ConstantVariable.GetNotFoundImage()));

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new BitmapImage();
            }

        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
 
}