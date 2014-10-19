using System;

namespace OptimusPrime
{
    public interface IOutputWriter
    {
        void WriteErrorLine(string format, params object[] args);

        void WriteLine(string format, params object[] args);

        void WriteLine(Enum color, string format, params object[] args);
    }
}