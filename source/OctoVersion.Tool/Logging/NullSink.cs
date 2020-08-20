using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Tool.Logging
{
    internal class NullSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
        }
    }
}