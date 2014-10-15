using System.Linq;
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
    public class NordiskFilmListener : IListener
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
                case "NF":
                    return GetDistributor(pCommand.RemoveCommand());
                default:
                    return string.Empty;
            }
        }

        private string GetDistributor(string pSearchParam)
        {
            var vUrl = "http://www.discshop.se/search.php?q=" + pSearchParam.Replace(" ", "+");
            var vWc = new WebClient();
            var vDoc = new HtmlDocument();
            vDoc.Load(vWc.OpenRead(vUrl), Encoding.GetEncoding("ISO-8859-1"));

            var vMetaTag = vDoc.DocumentNode.SelectSingleNode("//div[@class='pi']");
            string vReturnString;

            if (vMetaTag != null)
            {
                var vGetTitle = Regex.Matches(vMetaTag.InnerHtml, "title=\".*?\">(.*?)</a>");
                var vTitle = vGetTitle[0].Groups[1].ToString();

                var vGetUrl = Regex.Matches(
                    vMetaTag.InnerHtml,
                    "(http|ftp|https)://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&\\*\\(\\)_\\-\\=\\+\\\\/\\?\\.\\:\\;\\'\\,]*)?");
                var vNewUrl = vGetUrl[0].Value;

                vDoc.Load(vWc.OpenRead(vNewUrl), Encoding.GetEncoding("ISO-8859-1"));

                var vTag = vDoc.DocumentNode.SelectNodes("//div[@class='info_180']");

                var vNode = vTag[1];

                var vMs2 = Regex.Matches(vNode.InnerHtml, "http://.*?\">(.*?)</a>");

                var vDist = vMs2.Count > 0 ? vMs2[0].Groups[1].ToString() : "N/A";

                vReturnString = "Title: " + vTitle + OpConstants.NewLineChar + "Distributor: " + vDist;
            }
            else
            {
                vReturnString = "Nothing found.";
            }

            return vReturnString;
        }
    }
}
