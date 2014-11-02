using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace OptimusPrime.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        private const string UrlRegExp =
            @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+";

        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 Safari/537.36";

        public IEnumerable<Uri> ExtractUris(string message)
        {
            var match = Regex.Match(message, UrlRegExp);
            if (match.Success) yield return new Uri(match.Value);
            while ((match = match.NextMatch()).Success)
                yield return new Uri(match.Value);
        }

        public string GetTitleFromUrl(Uri uri, string charset = null)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);
                request.UserAgent = UserAgent;
                var response = (HttpWebResponse)request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    var doc = new HtmlDocument();
                    if (!string.IsNullOrEmpty(charset))
                    {
                        doc.Load(stream, Encoding.GetEncoding(charset), true);
                    }
                    else if (!string.IsNullOrEmpty(response.CharacterSet))
                    {
                        doc.Load(stream, Encoding.GetEncoding(response.CharacterSet), true);
                    }
                    else
                    {
                        doc.Load(stream, true);
                    }
                    var titleNode = doc.DocumentNode.SelectSingleNode("//title");
                    return titleNode == null ? string.Empty : HttpUtility.HtmlDecode(titleNode.InnerText).Replace("\n", "");
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed processing {0}", uri);
                Console.Error.WriteLine(e);
                return string.Empty;
            }
        }
    }
}