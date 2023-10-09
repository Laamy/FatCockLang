using System;

class FC_ConsoleApi
{
    public object WriteLine(object[] args)
    {
        if (args.Length < 1)
            throw new Exception("ConsoleApi.WriteLine() requires at least one argument");

        foreach (object arg in args)
        {
            Console.WriteLine(arg);
        }

        return null;
    }

    public object Write(object[] args)
    {
        if (args.Length < 1)
            throw new Exception("ConsoleApi.Write() requires at least one argument");

        foreach (object arg in args)
        {
            Console.Write(arg);
        }

        return null;
    }
}