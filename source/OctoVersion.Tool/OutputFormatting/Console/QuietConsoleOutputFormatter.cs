using OctoVersion.Core;
using OctoVersion.Tool.Logging;
using Serilog.Core;

namespace OctoVersion.Tool.OutputFormatting.Console
{
    internal class QuietConsoleOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new NullSink();

        public void Write(StructuredOutput structuredOutput)
        {
            System.Console.WriteLine(structuredOutput);
        }
    }
}