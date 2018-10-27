using DominatorHouseCore.ViewModel;
using NLog;
using NLog.Common;
using NLog.Targets;
using Unity;

namespace DominatorHouseCore.Diagnostics.LogHelper
{
    [Target("NlogUiTarget")]
    public class NlogUiTarget : TargetWithLayout
    {
        private readonly ILogViewModel _logViewModel;

        public NlogUiTarget()
        {
            _logViewModel = IoC.Container.Resolve<ILogViewModel>();
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
            _logViewModel.Add(logEvent.FormattedMessage, logEvent.Level);

        }
    }
}
