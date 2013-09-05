using System.Linq;
using OptimusPrime.Specifications;

namespace OptimusPrime.Shared
{
    public static class ExtensionMethods
    {
        public static string RemoveTrigger(this string pCommand)
        {
            return new CommandSpec().IsSatisfiedBy(pCommand) ? pCommand.Remove(0, 1) : pCommand;
        }

        public static string RemoveCommand(this string pCommand)
        {
            var arr = pCommand.Split(' ').Skip(1).ToArray();
            return string.Join(" ", arr);
        }

        public static string CommandTrimToUpper(this string pCommand)
        {
            if (!string.IsNullOrEmpty(pCommand))
            {
                return pCommand.Split(' ')[0].Trim().ToUpper();
            }
            return string.Empty;
        }
    }
}
