using System;
using System.Collections.Generic;
using System.Linq;
using ApogeeDev.IdServer.Helpers.Utilities;

namespace ApogeeDev.IdServer
{
    public static class GenericExtensions
    {
        public static bool IsNull<TSource>(this TSource source) where TSource : class
        {
            return source == null;
        }
        public static bool IsNotNull<TSource>(this TSource source) where TSource : class
        {
            return !source.IsNull();
        }
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second, Func<TSource, object> fieldSelector)
        {
            return first.Except(second, new GenericEqualityComparer<TSource>(fieldSelector));
        }
    }
}