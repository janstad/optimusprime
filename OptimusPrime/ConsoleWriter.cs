using System;

namespace OptimusPrime
{
    public class ConsoleWriter : IOutputWriter
    {
        public void WriteErrorLine(string format, params object[] args)
        {
            Console.Error.WriteLine(format, args);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void WriteLine(Enum color, string format, params object[] args)
        {
            if (!Enum.IsDefined(typeof(ConsoleColor), color))
            {
                throw new ArgumentException("Supplied color is not valid.");
            }
            Console.ForegroundColor = (ConsoleColor)color;
            WriteLine(format, args);
            Console.ResetColor();
        }
    }
}