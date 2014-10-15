using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{

    public class KolliListener : IListener
    {

        public const string cUrl = "http://www.posten.se/sv/Kundservice/Sidor/Sok-brev-paket.aspx?view=item&itemid={0}";

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
                case "KOLLI":
                    return GetKolliInfo(pMsg.Body);
                default:
                    return string.Empty;
            }
        }

        private string GetKolliInfo(string pKolli)
        {
            var kolli = string.Empty;
            var kolliArray = pKolli.Split(' ');
            
            if (kolliArray.Length <= 1)
            {
                return string.Empty;
            }

            kolli = pKolli.RemoveCommand();

            var statusList = PostenAPI.Posten.GetDeliveryStatuses(kolli);
            var sb = new StringBuilder();

            foreach (var status in statusList)
            {
                sb.Append(status.Date.ToString("yyyy-MM-dd hh:mm"));
                sb.Append(" - ");
                sb.Append(status.Status);
                sb.Append(OpConstants.NewLineChar);
            }

         return sb.ToString();
        }

    }
}
