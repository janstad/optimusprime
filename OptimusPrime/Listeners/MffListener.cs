using System.Linq;
using System.Net;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;
using HtmlAgilityPack;

namespace OptimusPrime.Listeners
{
    public class MffListener : IListener
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
                case "MFF":
                    return GetNextMatchInfo();
                default:
                    return string.Empty;
            }
        }

        private static string GetNextMatchInfo()
        {
            var wc = new WebClient();
            var doc = new HtmlDocument();

            doc.Load(wc.OpenRead("http://www.mff.se"), true);


            var game =
                doc.DocumentNode.Descendants("h5")
                .FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Split(' ')
                                                                       .Any(y => y.Equals("barometric-header"))).InnerText.Trim();

            var date = 
                doc.DocumentNode.Descendants("p")
                .FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Split(' ')
                                                                       .Any(y => y.Equals("barometric-paragraph"))).InnerText.Trim();

            var tickets =
                doc.DocumentNode.Descendants("div")
                .FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Split(' ')
                                                                       .Any(y => y.Equals("barometric-text-inner"))).InnerText.Trim();

            tickets = tickets.Replace("&nbsp;", " ").Trim();


            return string.Format("Nästa hemmamatch: {0} |\\n{1} - {2}", game, date, tickets);
        }




    }
}
