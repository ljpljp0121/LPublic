using System;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public class InitOnLoadAttribute : Attribute
{
}

public static class ClientAttributes
{
    public static void ExecuteInitOnLoadMethods()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract);
            foreach (var type in types)
            {
                var methods = type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttributes(typeof(InitOnLoadAttribute), false).Any());
                foreach (var method in methods)
                {
                    method.Invoke(null, null);
                }
            }
        }
    }
}