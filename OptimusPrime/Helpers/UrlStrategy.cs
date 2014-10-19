using System;

namespace OptimusPrime.Helpers
{
    public abstract class UrlStrategy : IUrlStrategy
    {
        public Uri Uri { get; set; }

        public abstract string ExtractInformationFromUrl();
    }
}