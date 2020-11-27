using System;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;

namespace OctoVersion.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>(args);

            var runner = new OctoVersionRunner(appSettings, configuration);
            runner.Run(out _);
        }
    }
}