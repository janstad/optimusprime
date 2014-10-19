using System;

namespace OptimusPrime.Helpers
{
    public class UrlStrategyImgur : UrlStrategy
    {
        private readonly IHttpHelper _httpHelper;

        public UrlStrategyImgur(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public override string ExtractInformationFromUrl()
        {
            return _httpHelper.GetTitleFromUrl(
                new Uri(
                    Uri.AbsoluteUri
                    .Replace(".jpg", string.Empty)
                    .Replace(".jpeg", string.Empty)
                    .Replace(".gif", string.Empty)
                    .Replace(".png", string.Empty)
                    ));
        }
    }
}