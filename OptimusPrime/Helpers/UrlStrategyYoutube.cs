using System;

namespace OptimusPrime.Helpers
{
    public class UrlStrategyYoutube : UrlStrategy
    {
        private readonly IHttpHelper _httpHelper;

        public UrlStrategyYoutube(Uri uri, IHttpHelper httpHelper)
            : base(uri)
        {
            _httpHelper = httpHelper;
        }

        public override string ExtractInformationFromUrl()
        {
            return _httpHelper.GetTitleFromUrl(Uri).Replace(" - YouTube", string.Empty);
        }
    }
}