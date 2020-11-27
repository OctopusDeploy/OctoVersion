using System;
using Serilog.Core;

namespace OctoVersion.Core
{
    public interface IOutputFormatter
    {
        ILogEventSink LogSink { get; }
        void Write(OctoVersionInfo octoVersionInfo);
    }
}