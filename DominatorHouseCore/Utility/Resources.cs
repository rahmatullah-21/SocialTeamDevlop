using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace DominatorHouseCore.Utility
{
    internal class Resources
    {

        private static ResourceManager _resourceMan;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceMan == null)
                    _resourceMan = new ResourceManager("DominatorHouseCore.Resources", typeof(Resources).Assembly);
                return _resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture { get; set; }

        internal static string UserAccountEditPasswordNotValue => ResourceManager.GetString(nameof(UserAccountEditPasswordNotValue), Culture);
    }
}
