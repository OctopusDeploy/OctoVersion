using System;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Exceptions;
using Serilog;

namespace OctoVersion.Tool;

class Program
{
    static int Main(string[] args)
    {
        var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>(args);

        var runner = new OctoVersionRunner(appSettings, configuration);
        try
        {
            runner.Run(out _);
        }
        catch (ControlledFailureException ex)
        {
            Log.Error("{Message}", ex.Message);
            return 1;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{Message}", ex.Message);
            return 1;
        }

        return 0;
    }
}