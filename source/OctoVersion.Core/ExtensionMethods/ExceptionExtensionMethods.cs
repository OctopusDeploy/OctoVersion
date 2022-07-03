using System;

namespace OctoVersion.Core.ExtensionMethods;

public static class ExceptionExtensionMethods
{
    public static TException WithData<TException>(this TException exception, string key, object value) where TException : Exception
    {
        exception.Data[key] = value;
        return exception;
    }
}