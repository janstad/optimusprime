using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OptimusPrime.Helpers
{
    class HttpHelper
    {
        private const string UrlRegExp =
            @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+";
        public IEnumerable<Uri> ExtractUris(string message)
        {
            var match = Regex.Match(message, UrlRegExp);
            if (match.Success) yield return new Uri(match.Value);
            while ((match = match.NextMatch()).Success)
                yield return new Uri(match.Value);
        }
    }
}
