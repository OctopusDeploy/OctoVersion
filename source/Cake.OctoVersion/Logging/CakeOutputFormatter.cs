using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using OctoVersion.Core;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace Cake.OctoVersion.Logging
{
    public class CakeOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; }

        readonly ICakeContext context;
        
        public CakeOutputFormatter(ICakeContext context)
        {
            this.context = context;
            LogSink = new CakeSink(context);
        }
        
        public void Write(OctoVersionInfo octoVersionInfo)
        {
            context.Log.Information(octoVersionInfo);
        }
    }
}