using System;
using System.Collections.Generic;

namespace OptimusPrime.Helpers
{
    public interface IHttpHelper
    {
        IEnumerable<Uri> ExtractUris(string message);

        string GetTitleFromUrl(Uri uri, string charset = null);
    }
}