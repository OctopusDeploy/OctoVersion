using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using OctoVersion.Core;
using Serilog;
using Serilog.Core;

namespace Cake.OctoVersion.Logging
{
    public class CakeOutputFormatter : IOutputFormatter
    {
        readonly ICakeContext context;
        readonly ILogEventSink logSink;

        public CakeOutputFormatter(ICakeContext context)
        {
            this.context = context;
            logSink = new CakeSink(context);
        }

        public string Name => "Cake";

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            context.Log.Information(octoVersionInfo);
        }

        public bool MatchesRuntimeEnvironment()
        {
            return false;
        }

        public void ConfigureLogSink(LoggerConfiguration lc)
        {
            lc.WriteTo.Sink(logSink);
        }
    }
}
