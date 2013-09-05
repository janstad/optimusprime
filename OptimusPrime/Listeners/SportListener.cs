using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

            switch (pCommand.CommandTrimToUpper())
            {
                case "FOTBOLL":
                    return GetGames("Fotboll");
                case "HOCKEY":
                    return GetGames("Ishockey");
                case "TENNIS":
                    return GetGames("Tennis");
                case "SPEEDWAY":
                    return GetGames("Speedway");
                case "BASKET":
                    return GetGames("Basket");
                case "AMFOTBOLL":
                    return GetGames("Amerikansk fotboll");
                case "HANDBOLL":
                    return GetGames("Handboll");
                case "F1":
                    return GetGames("Formel 1");
                case "GOLF":
                    return GetGames("Golf");
                default:
                    return string.Empty;
            }
        }



        private string GetGames(string pSport)
        {
            try
            {
                var wc = new WebClient();
                var doc = new HtmlDocument();
                doc.Load(wc.OpenRead("http://www.tvmatchen.nu"), Encoding.UTF8);

                var metaTags = doc.DocumentNode.SelectNodes("//div[@class='match-info clearfix']");
                var matchList = new List<string>();

                foreach (var node in metaTags)
                {

                    var matches = Regex.Matches(node.InnerHtml, "title=\"(.*?)\">");
                    var sport = matches[0].Groups[1].ToString();

                    //Code for TV-channel
                    var vChannel = matches[1].Groups[1].ToString();
                    if (matches.Count > 2)
                        vChannel = vChannel + "/" + matches[2].Groups[1];

                    matches = Regex.Matches(node.InnerHtml, "content=\"(.*?)T");
                    var date = matches[0].Groups[1].ToString();
                    if (DateTime.Parse(date) != DateTime.Now.Date)
                        if (DateTime.Parse(date) != DateTime.Now.Date.AddDays(1))
                            break;


                    if (sport.Equals(pSport))
                    {
                        matches = Regex.Matches(node.InnerHtml, "\"field-content\">(.*?)</span>");
                        var gameName = matches[1].Groups[1].ToString();

                        matches = Regex.Matches(node.InnerHtml, "([0-9][0-9]:[0-5][0-9])</span>");
                        var time = matches[0].Groups[1].ToString();

                        if (DateTime.Parse(date) == DateTime.Now.Date || DateTime.Parse(date) == DateTime.Now.Date.AddDays(1) &&
                            DateTime.Parse(time).TimeOfDay < DateTime.Parse("05:00").TimeOfDay)
                        {
                            matches = Regex.Matches(node.InnerHtml, "class=\"league\">\\n(.*?)<");

                            var league = matches[0].Groups[1].ToString().Trim();

                            if (string.IsNullOrEmpty(league))
                            {
                                matches = Regex.Matches(node.InnerHtml, @"class=""field-content"">(.*?)<");

                                if (matches.Count != 0)
                                    league = matches[3].Groups[1].ToString();
                            }

                            var full = time + ": " + gameName + " (" + league + ") - " + vChannel;
                            matchList.Add(full);
                        }
                    }
                }

                if (matchList.Count > 0)
                {
                    return string.Join("|", matchList.ToArray());
                }

                return "-";
            }
            catch (Exception)
            {
                return "-";
            }
        }
    }
}
