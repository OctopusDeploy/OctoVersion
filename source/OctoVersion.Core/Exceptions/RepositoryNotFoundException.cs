using System;

namespace OctoVersion.Core.Exceptions;

public class RepositoryNotFoundException : ControlledFailureException
{
    public RepositoryNotFoundException(string message) : base(message)
    {
    }

    public RepositoryNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}