using System;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Core.Logging;

class NullSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
    }
}