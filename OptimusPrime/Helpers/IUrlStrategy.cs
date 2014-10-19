using System;

namespace OptimusPrime.Helpers
{
    public interface IUrlStrategy
    {
        Uri Uri { get; set; }

        string ExtractInformationFromUrl();
    }
}