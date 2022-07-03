using System;

namespace OctoVersion.Core.ExtensionMethods;

public static class ObjectExtensionMethods
{
    public static TOutput Apply<TInput, TOutput>(this TInput target, Func<TInput, TOutput> func)
    {
        return func(target);
    }

    public static TInput Apply<TInput>(this TInput target, Action<TInput> action)
    {
        action(target);
        return target;
    }
}