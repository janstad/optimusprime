using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
 public   class StringListener : IListener
     {
      const string CTeamSpeak = "Mother Flankers TeamSpeak 3 server!\nip: maddah.se\npw: buchannon\nhttp://www.teamspeak.com/?page=downloads";

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
              case "TS":
                  return CTeamSpeak;
              default:
                  return string.Empty;
          }
      }

    }
}
