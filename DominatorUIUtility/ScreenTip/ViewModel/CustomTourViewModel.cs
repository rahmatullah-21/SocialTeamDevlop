using DominatorUIUtility.ScreenTip.ScreenTipMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility.ScreenTip.ViewModel
{
    public class CustomTourViewModel : TourViewModel
    {
        public CustomTourViewModel(IScreenTipRun run)
            : base(run) { }

        public string CustomProperty => "S T E P S";
    }
}
