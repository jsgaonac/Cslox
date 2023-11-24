namespace Cslox.Lang;

public class Log
{
    public static void Error(int line, string msg)
    {
        Console.WriteLine(msg);
    }
}