using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public class LoggerHelper
    {
        DateTime CurrentTime { get; set; }
        string ApplicationName { get; set; }
        string LogType { get; set; }
        string LogDiscription { get; set; }
        public ObservableCollection<LoggerHelper> LstInfoLoggerHelper { get; set; } = new ObservableCollection<LoggerHelper>();
        public ObservableCollection<LoggerHelper> LstErrorLoggerHelper { get; set; } = new ObservableCollection<LoggerHelper>();

    }
}
