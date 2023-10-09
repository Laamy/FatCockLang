using System.Collections.Generic;
using System;

class Environment
{
    private Dictionary<string, object> variables = new Dictionary<string, object>();
    private Dictionary<string, Func<object[], object>> functions = new Dictionary<string, Func<object[], object>>();
    private Dictionary<string, Environment> importedModules = new Dictionary<string, Environment>();

    public Environment(Environment inherit)
    {
        variables = inherit.variables;
        functions = inherit.functions;
        importedModules = inherit.importedModules;
    }

    public Environment() { }

    public Dictionary<string, Environment> GetModules()
    {
        return importedModules;
    }

    public object GetVariable(string name)
    {
        if (variables.ContainsKey(name))
        {
            return variables[name];
        }
        else
        {
            throw new Exception($"Variable {name} does not exist");
        }
    }

    public void SetVariable(string name, object value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
        }
        else
        {
            variables.Add(name, value);
        }
    }

    public void DeclareFunction(string name, Func<object[], object> function)
    {
        if (functions.ContainsKey(name))
        {
            functions[name] = function;
        }
        else
        {
            functions.Add(name, function);
        }
    }

    public Func<object[], object> GetFunction(string name)
    {
        if (functions == null)
            return null;

        if (functions.ContainsKey(name))
        {
            return functions[name];
        }
        else
        {
            return null;
        }
    }

    public void ImportModule(string name, Environment module)
    {
        if (importedModules.ContainsKey(name))
        {
            importedModules[name] = module;
        }
        else
        {
            importedModules.Add(name, module);
        }
    }

    public Environment GetIModule(string name)
    {
        if (importedModules.ContainsKey(name))
        {
            return importedModules[name];
        }
        else
        {
            throw new Exception($"Module {name} does not exist");
        }
    }
}