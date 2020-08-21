using System;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Tool.Logging
{
    class NullSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
        }
    }
}