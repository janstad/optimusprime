using System;

namespace OptimusPrime.Helpers
{
    public class UrlStrategyDefault : UrlStrategy
    {
        private readonly IHttpHelper _httpHelper;

        public UrlStrategyDefault(Uri uri, IHttpHelper httpHelper)
            : base(uri)
        {
            _httpHelper = httpHelper;
        }

        public override string ExtractInformationFromUrl()
        {
            return _httpHelper.GetTitleFromUrl(Uri);
        }
    }
}