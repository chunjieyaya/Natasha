﻿using System.Reflection;
using System.Runtime.Loader;


public static class NatashaAssemblyExtension
{


    public static NatashaReferenceDomain GetDomain(this Assembly assembly)
    {

        var assemblyDomain = AssemblyLoadContext.GetLoadContext(assembly);
        if (assemblyDomain == AssemblyLoadContext.Default)
        {
            return NatashaReferenceDomain.DefaultDomain!;
        }
        return (NatashaReferenceDomain)assemblyDomain!;

    }


    public static void DisposeDomain(this Assembly assembly)
    {

        var domain = GetDomain(assembly);
        if (domain.Name != "Default")
        {
            domain.Dispose();
        }

    }

}

