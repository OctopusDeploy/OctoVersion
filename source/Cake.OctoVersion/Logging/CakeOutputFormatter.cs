using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using OctoVersion.Core;
using Serilog.Core;

namespace Cake.OctoVersion.Logging
{
    public class CakeOutputFormatter : IOutputFormatter
    {
        readonly ICakeContext context;

        public CakeOutputFormatter(ICakeContext context)
        {
            this.context = context;
            LogSink = new CakeSink(context);
        }

        public ILogEventSink LogSink { get; }
        public string Name => "Cake";

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            context.Log.Information(octoVersionInfo);
        }

        public bool MatchesRuntimeEnvironment() => false;
    }
}
