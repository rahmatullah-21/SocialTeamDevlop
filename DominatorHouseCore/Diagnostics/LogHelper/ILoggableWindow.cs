using NLog;

namespace DominatorHouseCore.LogHelper
{
    public interface ILoggableWindow
    {
        void LogText(string log, LogLevel logLevel);
    }
}
