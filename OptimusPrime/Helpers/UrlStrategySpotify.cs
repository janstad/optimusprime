﻿using System;

namespace OptimusPrime.Helpers
{
    public class UrlStrategySpotify : UrlStrategy
    {
        private readonly IHttpHelper _helper;

        public UrlStrategySpotify(Uri uri, IHttpHelper helper)
            : base(uri)
        {
            _helper = helper;
        }

        public override string ExtractInformationFromUrl()
        {
            return _helper.GetTitleFromUrl(Uri, "utf-8").Replace("Spotify Web Player - ", string.Empty);
        }
    }
}