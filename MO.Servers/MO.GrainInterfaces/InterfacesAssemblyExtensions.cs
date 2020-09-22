using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MO.GrainInterfaces
{
    public static class InterfacesAssemblyExtensions
    {
        public static ICollection<Assembly> AddInterfaces(this ICollection<Assembly> assemblies)
        {
            assemblies.Add(typeof(InterfacesAssemblyExtensions).Assembly);
            return assemblies;
        }
    }
}
