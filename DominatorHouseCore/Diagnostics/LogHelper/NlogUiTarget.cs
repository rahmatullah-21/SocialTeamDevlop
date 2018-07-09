using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;

namespace DominatorHouseCore.LogHelper
{
    public class NlogUiTarget : Target
    {
        public Action<string, LogLevel> Log = delegate { };

        public NlogUiTarget(string name, LogLevel level)
        {
            LogManager.Configuration.AddTarget(name, this);

            // This will ensure that exsiting rules are not overwritten
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", level, this));
            LogManager.Configuration.Reload();
        }

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (var logEvent in logEvents)
            {
                Write(logEvent);
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            //if (logEvent.Level != LogLevel.Error)
            //    Log(logEvent.FormattedMessage, false);
            //else
            //    Log(logEvent.FormattedMessage, true);
            
                Log(logEvent.FormattedMessage, logEvent.Level);
          
        }
    }
}
