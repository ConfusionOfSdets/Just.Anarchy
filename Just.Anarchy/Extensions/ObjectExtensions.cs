using System;
using System.Collections.Generic;
using System.Text;

namespace Just.Anarchy.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNotOfType<TDestType>(this object candidate) => !(candidate is TDestType);

        public static bool IsOfType<TDestType>(this object candidate) => candidate is TDestType;
    }
}
