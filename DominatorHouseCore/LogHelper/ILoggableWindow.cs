using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.LogHelper
{
    public interface ILoggableWindow
    {
        void LogText(string log, bool error);
    }
}
