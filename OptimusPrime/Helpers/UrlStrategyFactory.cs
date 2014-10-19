using System;

namespace OptimusPrime.Helpers
{
    public class UrlStrategyFactory : IUrlStrategyFactory
    {
        private readonly IHttpHelper _httpHelper;

        public UrlStrategyFactory(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public IUrlStrategy Create(Uri uri)
        {
            switch (uri.Host.ToLower().Replace("www.", string.Empty))
            {
                case "imdb.com":
                    return new UrlStrategyImdb { Uri = uri };

                case "i.imgur.com":
                case "imgur.com":
                    return new UrlStrategyImgur(_httpHelper)
                    {
                        Uri = new Uri(
                            uri.AbsoluteUri.Replace(
                                "i.imgur.com",
                                "imgur.com"))
                    };
            }
            return new UrlStrategyDefault(_httpHelper) { Uri = uri };
        }
    }
}