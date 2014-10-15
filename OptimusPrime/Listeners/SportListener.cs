using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class SportListener : IListener
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

            var commands = pCommand.Split('+');
            var plusDays = 0;

            if (commands.Length > 1 && int.TryParse(commands[1], out plusDays))
            {
                if (plusDays > 6) plusDays = 0;
            }
            

            switch (commands[0].CommandTrimToUpper())
            {
                case "FOTBOLL":
                    return GetGames("Fotboll", plusDays);
                case "HOCKEY":
                    return GetGames("Ishockey", plusDays);
                case "TENNIS":
                    return GetGames("Tennis", plusDays);
                case "SPEEDWAY":
                    return GetGames("Speedway", plusDays);
                case "BASKET":
                    return GetGames("Basket", plusDays);
                case "AMFOTBOLL":
                    return GetGames("Amerikansk fotboll", plusDays);
                case "HANDBOLL":
                    return GetGames("Handboll", plusDays);
                case "F1":
                    return GetGames("Formel 1", plusDays);
                case "GOLF":
                    return GetGames("Golf", plusDays);
                default:
                    return string.Empty;
            }
        }



        private static string GetGames(string pSport, int pPlusDays)
        {
            try
            {
                var matchList = new List<string>();
                var wc = new WebClient();
                var doc = new HtmlDocument();

                doc.Load(wc.OpenRead("http://www.tvmatchen.nu"), Encoding.UTF8);

                var days = doc.DocumentNode.SelectNodes("//ul[@class='match-list']");
                var today = days[pPlusDays];
                var todaysMatches = today.SelectNodes("li");

                foreach (var matchNode in todaysMatches)
                {
                    var match = matchNode.SelectSingleNode("div[@class='match-info clearfix']");

                    if (match == null) continue;

                    var sport = match.SelectSingleNode("div[@class='sport']").InnerText;

                    if (sport != pSport)
                    {
                        continue;
                    }

                    var vTime = match.SelectSingleNode("div[@class='time']").InnerText;

                    var vMatchName = match.SelectSingleNode("h3[@class='match-name']").InnerText;
                    var vLeagueNode = match.SelectSingleNode("div[@class='league']");
                    var vLeagueName = vLeagueNode.FirstChild.InnerText.Replace("\n", "").Trim();

                    if (string.IsNullOrEmpty(vLeagueName))
                    {
                        vLeagueName = vLeagueNode.InnerText.Replace("\n", "").TrimFull();
                    }

                    var channels = match.SelectSingleNode("div[@class='channel']");
                    var vChannelName = string.Empty;

                    foreach (var child in channels.ChildNodes.Where(child => child.Name == "img"))
                    {
                        if (!string.IsNullOrEmpty(vChannelName)) vChannelName += "/";
                        vChannelName += child.GetAttributeValue("title", string.Empty);
                    }

                    var full = string.Format("{0}: {1} ({2}) - {3}",
                                    vTime,
                                    vMatchName,
                                    vLeagueName,
                                    vChannelName);
                    matchList.Add(full);
                }

                if (matchList.Count <= 0) return "Nothing...";
                matchList.Insert(0, string.Format("----- {0} -----", 
                    DateTime.Today.AddDays(pPlusDays).DayOfWeek));
                return string.Join(OpConstants.NewLineChar, matchList.ToArray());
            }
            catch (Exception)
            {
                return "Nothing...";
            }
        }
    }
}
