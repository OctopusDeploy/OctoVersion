using System;

namespace OctoVersion.Core.Exceptions;

public class RepositoryIsShallowCloneException : ControlledFailureException
{
    public RepositoryIsShallowCloneException(string message) : base(message)
    {
    }

    public RepositoryIsShallowCloneException(string message, Exception innerException) : base(message, innerException)
    {
    }
}