using System.Reflection;
using OctoVersion.Core;
using OctoVersion.Tool.Logging;
using Serilog.Core;

namespace OctoVersion.Tool.OutputFormatting.Environment
{
    public class EnvironmentOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new NullSink();

        public void Write(StructuredOutput structuredOutput)
        {
            const string prefix = ConfigurationBootstrapper.EnvironmentVariablePrefix;

            var properties = structuredOutput.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var key = $"{prefix}{property.Name}";
                var value = property.GetValue(structuredOutput)?.ToString() ?? string.Empty;
                var line = $"{key}={value}";
                System.Console.WriteLine(line);
            }
        }
    }
}