using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using CommonServiceLocator;
using DominatorHouseCore.ViewModel;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace DominatorHouseCore.Diagnostics.LogHelper
{
    [Target("NlogUiTarget")]
    public class NlogUiTarget : TargetWithLayout
    {
        private readonly ILogViewModel _logViewModel;

        public NlogUiTarget()
        {
            _logViewModel = ServiceLocator.Current.GetInstance<ILogViewModel>();
        }

        //protected override void Write(IList<AsyncLogEventInfo> logEvents)
        //{
        //    foreach (var logEvent in logEvents)
        //    {
        //        Write(logEvent);
        //    }
        //}

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Action)(() =>
             _logViewModel.Add(logEvent.FormattedMessage, logEvent.Level))
            );
        }
    }
}
