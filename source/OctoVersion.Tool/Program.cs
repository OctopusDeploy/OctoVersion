using System;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using Serilog;

namespace OctoVersion.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>(args);

            var runner = new OctoVersionRunner(appSettings, configuration, (lc) => { });
            runner.Run(out _);
        }
    }
}