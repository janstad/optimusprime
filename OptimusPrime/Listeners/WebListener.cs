using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OptimusPrime.Interfaces;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class WebListener : IListener
    {
        private const string COmdbUrl = "http://www.omdbapi.com/?i=";

        public string Call(string pCommand, ChatMessage pMsg)
        {
            var urls = Regex.Match(pCommand, @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+");

            if (urls.Groups.Count == 0) return string.Empty;
            if (urls.Groups.Count == 1 && urls.Groups[0].Value == string.Empty) return string.Empty;

            var url = urls.Groups[0].Value;
            var uri = new Uri(url);
            var host = uri.Host.Replace("www.", "");

            switch (host.ToUpper().Trim())
            {
                case "IMDB.COM":
                    return GetImdbString(uri);
                //case "OPEN.SPOTIFY.COM":
                //    return GetSpotifyString(uri);
            }

            return GetUrlTitle(url);
        }

        private static string GetImdbString(Uri pUri)
        {
            using (var wc = new WebClient())
            {
                var stream = wc.OpenRead(COmdbUrl + (pUri.Segments[pUri.Segments.Length - 1]).Replace("/", ""));
                var sr = new StreamReader(stream);

                var json = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());

                if (json["Response"] == "False")
                {
                    return string.Empty;
                }

                var title = json["Title"];
                var year = json["Year"];
                var runtime = json["Runtime"];
                var genre = json["Genre"];
                var rating = json["imdbRating"];
                var votes = json["imdbVotes"];
                var actors = json["Actors"];

                return string.Format("{0} ({1}) {2} - {3}/10 ({4} votes)\n{5}\n{6}",
                                     title, //0
                                     year, //1
                                     runtime, //2
                                     rating, //3
                                     votes, //4
                                     genre, //5
                                     actors); //6
            }
        }

        private static string GetUrlTitle(string pUrl)
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
