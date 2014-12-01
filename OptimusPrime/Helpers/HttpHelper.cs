using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
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

        public string GetTitleFromUrl(Uri uri)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);
                request.UserAgent = UserAgent;
                var response = (HttpWebResponse)request.GetResponse();
                var ms = new MemoryStream();
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) return string.Empty;
                    stream.CopyTo(ms);
                }
                using (ms)
                {
                    ms.Position = 0;
                    var charset = Encoding.ASCII.EncodingName;
                    var doc = new HtmlDocument();
                    if (!string.IsNullOrEmpty(response.CharacterSet) && response.CharacterSet != null)
                    {
                        charset = (response.CharacterSet);
                    }
                    doc.Load(ms, Encoding.GetEncoding(charset), true);
                    var html = doc.DocumentNode.OuterHtml;
                    var charsetStart = html.IndexOf("charset=\"", StringComparison.InvariantCulture);
                    var offset = 0;
                    if (charsetStart <= 0)
                    {
                        charsetStart = html.IndexOf("charset=", StringComparison.InvariantCulture);
                        offset = 1;
                    }
                    if (charsetStart > 0)
                    {
                        charsetStart += 9 - offset;
                        var charsetEnd = html.IndexOfAny(new[] { ' ', '\"', ';' }, charsetStart);
                        var realCharset = html.Substring(charsetStart, charsetEnd - charsetStart);
                        if (!realCharset.Equals(charset))
                        {
                            ms.Position = 0;
                            doc.Load(ms, Encoding.GetEncoding(realCharset), false);
                        }
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