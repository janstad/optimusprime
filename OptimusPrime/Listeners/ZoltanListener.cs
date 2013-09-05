using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class ZoltanListener : IListener
    {
        public string Call(string pCommand, ChatMessage pMsg)
        {
            if (!new CommandSpec().IsSatisfiedBy(pCommand))
            {
                return string.Empty;
            }
            
            pCommand = pCommand.RemoveTrigger();

            switch (pCommand.CommandTrimToUpper())
            {
                case "ZOLTAN":
                    return "kuhle";
                default:
                    return string.Empty;
            }
        }
    }
}
