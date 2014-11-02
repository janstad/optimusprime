using System;
using System.Linq;

namespace OptimusPrime.Helpers
{
    public class UrlStrategyAftonbladet : UrlStrategy
    {
        private readonly IHttpHelper _helper;

        public UrlStrategyAftonbladet(Uri uri, IHttpHelper helper)
            : base(uri)
        {
            _helper = helper;
        }

        public override string ExtractInformationFromUrl()
        {
            return _helper.GetTitleFromUrl(Uri).Split('|').FirstOrDefault();
        }
    }
}