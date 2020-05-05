using System;
using System.Collections.Generic;
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
            
            var option = new List<string>() { "Sexual content", "Violent or repulsive content", "Hateful or abusive content", "Harmful dangerous acts", "Child abuse", "Promotes terrorism", "Spam or misleading", "Infringes my rights", "Captions issue" }[(int)values[0]];
            var subOption = string.Empty;

            var subOptionIndex = (int)values[1];
            
            switch((int)values[0])
            {
                case 0:
                    subOption = new List<string> { "Graphic sexual activity", "Nudity", "Suggestive, but without nudity", "Content involving minors", "Abusive title or description", "Other sexual content" }[subOptionIndex];
                    break;
                case 1:
                    subOption = new List<string> { "Adults fighting", "Physical attack", "Youth violence", "Animal abuse" }[subOptionIndex];
                    break;
                case 2:
                    subOption = new List<string> { "Promotes hatred or violence", "Abusing vulnerable individuals", "Bullying", "Abusive title or description" }[subOptionIndex];
                    break;
                case 3:
                    subOption = new List<string> { "Pharmaceutical or drug abuse", "Abuse of fire or explosives", "Suicide or self injury", "Other dangerous acts" }[subOptionIndex];
                    break;
                case 4:
                case 5:
                    break;
                case 6:
                    subOption = new List<string> { "Mass advertising", "Pharmaceutical drugs for sale", "Misleading text", "Misleading thumbnail", "Scams/fraud" }[subOptionIndex];
                    break;
                case 7:
                    subOption = new List<string> { "Infringes my copyright", "Invades my privacy", "Other legal claim" }[subOptionIndex];
                    break;
                case 8:
                    subOption = new List<string> { "Captions are missing (CVAA)", "Captions are inaccurate", "Captions are abusive" }[subOptionIndex];
                    break;
                default:
                    break;
            }
            
            return string.IsNullOrEmpty(subOption) ? $"{option}" : $"{option} [ {subOption} ]";
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
