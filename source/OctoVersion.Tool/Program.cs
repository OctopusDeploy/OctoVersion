using System;
using System.Diagnostics;
using OctoVersion.Core;

namespace OctoVersion.Tool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var versionCalculator = new VersionCalculatorFactory(args[0]);
            var version = versionCalculator.Create();
            sw.Stop();

            Console.WriteLine($"Calculating version took {sw.Elapsed}");
            Console.WriteLine(version);
        }
    }
}