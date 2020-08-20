using Serilog.Core;

namespace OctoVersion.Core
{
    public interface IOutputFormatter
    {
        ILogEventSink LogSink { get; }
        void Write(StructuredOutput structuredOutput);
    }
}