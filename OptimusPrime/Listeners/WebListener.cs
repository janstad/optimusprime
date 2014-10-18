using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OptimusPrime.Helpers;
using OptimusPrime.Interfaces;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class WebListener : IListener
    {
        private const string COmdbUrl = "http://www.omdbapi.com/?i=";

        public string Call(string pCommand, ChatMessage pMsg)
        {
            var urlInfo = new List<string>();
            var uris = new HttpHelper().ExtractUris(pCommand).ToList();
            if (uris.Count() > 3)
                return "I refuse to process more than three URLs at a time.";
            foreach (var uri in uris)
            {
                switch (uri.Host.ToLower())
                {
                    case "imdb.com":
                        urlInfo.Add(GetImdbString(uri));
                        break;
                    default:
                        urlInfo.Add(GetUrlTitle(uri.AbsoluteUri));
                        break;
                }
            }
            return string.Join("\n", urlInfo);
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
