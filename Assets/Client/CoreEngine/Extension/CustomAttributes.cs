using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public class InitOnLoadAttribute : Attribute
{
}

public class InitOnLoadMethodManager
{
    public static void ProcessInitOnLoadMethods(Type assemblyClassType)
    {
        Type[] types = assemblyClassType.Assembly.GetTypes();
        foreach (Type type in types)
        {
            MethodInfo[] info = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (MethodInfo property in info)
            {
                foreach (object attribute in property.GetCustomAttributes(false))
                {
                    if (attribute.GetType() == typeof(InitOnLoadAttribute))
                    {
                        try
                        {
                            property.Invoke(null, null);
                        }
                        catch (Exception e)
                        {
                            LogSystem.Error("{0}", e);
                        }
                    }
                }
            }
        }
    }
}