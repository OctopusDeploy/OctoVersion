using System;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Core.Logging
{
    public class NullSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
        }
    }
}