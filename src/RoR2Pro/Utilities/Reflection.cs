using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RoR2Pro.Utilities
{
    public static class Reflection
    {
        public static IEnumerable<Type> GetImplementingTypes<TAssembly, TTarget>()
        {
            return Assembly.GetAssembly(typeof(TAssembly)).GetTypes()
                .Where(t => typeof(TTarget).IsAssignableFrom(t) && t != typeof(TTarget) && !t.IsAbstract);
        }
    }
}
