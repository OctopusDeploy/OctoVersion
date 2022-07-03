using System;
using System.Linq;
using OctoVersion.Core.Configuration;

namespace OctoVersion.Core;

public class OutputFormattersProvider
{
    public IOutputFormatter[] GetFormatters(AppSettings appSettings)
    {
        var allFormatters = GetType()
            .Assembly.DefinedTypes
            .Where(t => typeof(IOutputFormatter).IsAssignableFrom(t))
            .Where(t => !t.IsInterface)
            .Where(t => !t.IsAbstract)
            .Select(t => (IOutputFormatter)Activator.CreateInstance(t, appSettings))
            .ToArray();

        var settings = appSettings.OutputFormats;

        if (appSettings.DetectEnvironment)
            settings = DiscoverOutputFormatFromRuntimeEnvironment(allFormatters);

        if (!settings.Any())
            settings = new[] { "Console" };

        return RequestedOutputFormatters(settings, allFormatters);
    }

    static IOutputFormatter[] RequestedOutputFormatters(string[] requestedOutputFormats, IOutputFormatter[] allFormatters)
    {
        var formatters = allFormatters
            .Where(formatter => requestedOutputFormats.Any(requestedFormatter => formatter.Name.Equals(requestedFormatter, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return formatters;
    }

    static string[] DiscoverOutputFormatFromRuntimeEnvironment(IOutputFormatter[] outputFormatters)
    {
        return outputFormatters
            .Where(formatter => formatter.MatchesRuntimeEnvironment())
            .Select(x => x.Name)
            .ToArray();
    }
}