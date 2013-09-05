using System;

namespace OptimusPrime.Specifications
{
    public class CommandSpec
    {
        public bool IsSatisfiedBy(string pCommand)
        {
            return pCommand.IndexOf("!", StringComparison.Ordinal) == 0;
        }
    }
}
