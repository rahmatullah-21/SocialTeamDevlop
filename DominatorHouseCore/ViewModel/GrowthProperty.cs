using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel
{
    public class GrowthProperty:BindableBase
    {
        private string _propertyName;
        private int _propertyValue;

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                if(_propertyName == value)
                    return;
                SetProperty(ref _propertyName, value);
            }
        }

        public int PropertyValue
        {
            get
            {
                return _propertyValue;
            }
            set
            {
                if(_propertyValue == value)
                    return;
                SetProperty(ref _propertyValue, value);
            }
        }
    }
}
