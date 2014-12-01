using System;

namespace OptimusPrime.Helpers
{
    public class UrlStrategyImgur : UrlStrategy
    {
        private readonly IHttpHelper _httpHelper;

        public UrlStrategyImgur(Uri uri, IHttpHelper httpHelper)
            : base(new Uri(
                uri.AbsoluteUri
                    .Replace("i.imgur.com", "imgur.com")
                    .Replace(".jpg", string.Empty)
                    .Replace(".jpeg", string.Empty)
                    .Replace(".gif", string.Empty)
                    .Replace(".png", string.Empty)
                ))
        {
            _httpHelper = httpHelper;
        }

        public override string ExtractInformationFromUrl()
        {
            return _httpHelper.GetTitleFromUrl(Uri);
        }
    }
}