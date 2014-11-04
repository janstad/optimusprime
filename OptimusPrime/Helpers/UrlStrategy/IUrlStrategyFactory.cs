using System;

namespace OptimusPrime.Helpers
{
    public interface IUrlStrategyFactory
    {
        IUrlStrategy Create(Uri uri);
    }
}