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
                    return new UrlStrategyImdb(uri);

                case "i.imgur.com":
                case "imgur.com":
                    return new UrlStrategyImgur(uri, _httpHelper);

                case "youtube.com":
                case "youtu.be":
                    return new UrlStrategyYoutube(uri, _httpHelper);
            }
            return new UrlStrategyDefault(uri, _httpHelper);
        }
    }
}