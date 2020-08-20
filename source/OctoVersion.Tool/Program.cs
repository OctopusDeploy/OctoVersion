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
            var versionCalculatorFactory = new VersionCalculatorFactory(args[0]);
            var calculator = versionCalculatorFactory.Create();
            var version = calculator.GetVersion();
            sw.Stop();

            Console.WriteLine($"Calculating version took {sw.Elapsed}");
            Console.WriteLine(version);
        }
    }
}