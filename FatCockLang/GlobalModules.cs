using System;
using System.Collections.Generic;
using System.Reflection;

class GlobalModules
{
    private static Dictionary<string, Environment> modules = new Dictionary<string, Environment>();

    public static void RegisterModule(string name, Environment module)
    {
        modules.Add(name, module);
    }

    public static Environment GetModule(string name)
    {
        if (modules.ContainsKey(name))
        {
            return modules[name];
        }
        return null;
    }

    public static Environment ClassToModule(Type classType)
    {
        Environment module = new Environment();

        MethodInfo[] info = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (MethodInfo method in info)
        {
            if (!method.IsSpecialName)
            {
                module.DeclareFunction(method.Name, args =>
                {
                    object instance = Activator.CreateInstance(classType);
                    return method.Invoke(instance, new object[] { args });
                });
            }
        }

        return module;
    }

    public static void Init()
    {
        // this should register console api from the C# class i wrote for it
        RegisterModule("ConsoleApi", ClassToModule(typeof(FC_ConsoleApi)));
    }
}