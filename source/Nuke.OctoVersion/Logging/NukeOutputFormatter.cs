using OctoVersion.Core;
using OctoVersion.Core.Logging;
using Serilog.Core;
using Logger = Nuke.Common.Logger;

namespace Nuke.OctoVersion
{
    public class NukeOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new NukeSink();
        
        public void Write(OctoVersionInfo octoVersionInfo)
        {
            Logger.Info(octoVersionInfo);
        }
    }
}