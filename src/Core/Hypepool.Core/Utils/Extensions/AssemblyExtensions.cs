using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hypepool.Core.Utils.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> DiscoverTypes(this Assembly assembly, Type type)
        {
            return (from t in assembly.GetExportedTypes()
                where type.IsAssignableFrom(t)
                where !t.IsAbstract
                where !t.IsGenericTypeDefinition
                select t).ToArray();
        }
    }
}
