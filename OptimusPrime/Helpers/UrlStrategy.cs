using System;

namespace OptimusPrime.Helpers
{
    public abstract class UrlStrategy : IUrlStrategy
    {
        internal Uri Uri;

        protected UrlStrategy(Uri uri)
        {
            Uri = uri;
        }

        public abstract string ExtractInformationFromUrl();
    }
}