using System;

namespace OctoVersion.Core
{
    public static class ObjectExtensionMethods
    {
        public static TOutput Apply<TInput, TOutput>(this TInput target, Func<TInput, TOutput> func)
        {
            return func(target);
        }
    }
}