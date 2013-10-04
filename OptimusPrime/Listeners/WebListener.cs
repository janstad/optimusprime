using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using HtmlAgilityPack;
using OptimusPrime.Helpers;
using OptimusPrime.Interfaces;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class WebListener : IListener
    {
        public string Call(string pCommand, ChatMessage pMsg)
        {
            var urls = Regex.Match(pCommand, @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+");

            if (urls.Groups.Count == 0) return string.Empty;
            if (urls.Groups.Count == 1 && urls.Groups[0].Value == string.Empty) return string.Empty;

            var url = urls.Groups[0].Value;
            var host = GetHostName(url);

            switch (host.ToUpper().Trim())
            {
                case "IMDB.COM":
                    return GetImdbString(url);
                //case "OPEN.SPOTIFY.COM":
                //    return GetSpotifyString(url);
            }

            return GetUrlTitle(url);
        }

        private string GetHostName(string pUrl)
        {
            var uri = new Uri(pUrl);
            return uri.Host.Replace("www.", "");
        }

        private string GetSpotifyString(string pUrl)
        {



            return string.Empty;
        }

        private string GetImdbString(string pUrl)
        {
            var title = GetUrlTitle(pUrl);
            var rating = GetImdbRating(pUrl);

            return string.Format(@"{0}|\n{1}", title, rating);
        }

        private string GetImdbRating(string pUrl)
        {
            try
            {
                HtmlDocument doc;
                using (var wc = new WebClient())
                {
                    doc = new HtmlDocument();
                    doc.Load(wc.OpenRead(pUrl), true);
                }

                var metaTag = doc.DocumentNode.SelectSingleNode("//div[@class='star-box-details']");


                if (metaTag != null)
                {
                    var vResult = metaTag.InnerText.Substring(0, metaTag.InnerText.IndexOf("users", StringComparison.Ordinal) + "users".Length);
                    return Regex.Replace(vResult, @"\s+", " ");
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetUrlTitle(string pUrl)
        {
            try
            {
                HtmlDocument doc;
                using (var wc = new WebClient())
                {
                    doc = new HtmlDocument();
                    doc.Load(wc.OpenRead(pUrl), true);
                }

                var titleNode = doc.DocumentNode.SelectSingleNode("//title");

                if (titleNode == null) return string.Empty;
                var title = HttpUtility.HtmlDecode(titleNode.InnerText);

                if (title.Contains("\n")) title = title.Replace("\n", "");

                return title;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
