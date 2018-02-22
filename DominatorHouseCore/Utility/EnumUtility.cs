using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Utility
{
    public class EnumUtility
    {

        public static List<ActivityType> GetEnums( string requiredDescriptionData) 
        {
            // gets the Type that contains all the info required    
            // to manipulate this type    
            Type enumType = typeof(ActivityType);

            var enumValues = Enum.GetValues(typeof(ActivityType));

            return (
                from ActivityType value in enumValues
                let memberInfo = enumType.GetMember(value.ToString()).First()
                let descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>()
                where descriptionAttribute != null && descriptionAttribute.Description.Contains(requiredDescriptionData)
                select value)
                .ToList();
        }
    }
}
