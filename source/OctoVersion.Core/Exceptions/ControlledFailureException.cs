using System;

namespace OctoVersion.Core.Exceptions;

/// <summary>
/// A controlled failure is one where the task at hand must be aborted
/// but we do not need to reveal additional stack trace information to
/// the user,
/// </summary>
/// <remarks>
/// ONLY throw this exception if you're 100% certain that a stack trace
/// won't be useful to the user (or us) when they try to diagnose the issue.
/// </remarks>
public class ControlledFailureException : Exception
{
    public ControlledFailureException(string message)
        : base(message)
    {
    }

    public ControlledFailureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}