using System;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class RandomListener : IListener
    {
        public string Call(string pCommand, ChatMessage pMsg)
        {
            if (new CommandSpec().IsSatisfiedBy(pCommand)) //Command?
            {
                pCommand = pCommand.RemoveTrigger();
            }
            else
            {
                return string.Empty;
            }

            switch (pCommand.CommandTrimToUpper())
            {
                case "R":
                case "RANDOM":
                    return GetRandom(pCommand.RemoveCommand().Split(' '));
                default:
                    return string.Empty;
            }
        }

        private string GetRandom(string[] pCommands)
        {
            var vRnd = new Random();
            if (pCommands.Length > 1)
            {
                return pCommands[vRnd.Next(1, pCommands.Length)];
            }
            
            if(pCommands.Length == 1 && pCommands[0] != string.Empty)
            {
                return pCommands[0];
            }
            return "Not enought arguments...";
        }
    }
}
