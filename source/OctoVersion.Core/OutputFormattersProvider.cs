using System;
using System.Linq;

namespace OctoVersion.Core
{
    public class OutputFormattersProvider
    {
        public IOutputFormatter[] GetFormatters(string[] outputFormatterNames)
        {
            var allFormatters = GetType().Assembly.DefinedTypes
                .Where(t => typeof(IOutputFormatter).IsAssignableFrom(t))
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsAbstract)
                .Select(t => (IOutputFormatter)Activator.CreateInstance(t))
                .ToArray();

            var formatters = allFormatters
                .Where(f => outputFormatterNames.Any(n =>
                    f.GetType().Name.Equals($"{n}OutputFormatter", StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            return formatters;
        }
    }
}