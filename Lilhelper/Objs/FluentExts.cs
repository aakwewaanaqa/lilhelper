using System;

namespace Lilhelper.Objs
{
    public static class FluentExts
    {
        public static T Let<T>(this T t, Func<T, T> func)
        {
            return func(t);
        }

        public static U So<T, U>(this T t, Func<T, U> func)
        {
            return func(t);
        }
    }
}