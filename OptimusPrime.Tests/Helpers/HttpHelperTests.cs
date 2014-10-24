using NUnit.Framework;
using OptimusPrime.Helpers;
using System;
using System.Linq;

namespace OptimusPrime.Tests.Helpers
{
    [TestFixture]
    public class HttpHelperTests
    {
        private HttpHelper _target;

        [TestCase("", 0)]
        [TestCase("Lorem ipsum dolor sit amet, consectetur adipiscing elit. http://www.lipsum.com", 1)]
        [TestCase("http://www.lipsum.com Lorem ipsum dolor sit amet, consectetur adipiscing elit. http://www.lipsum.com/", 2)]
        [TestCase("http://www.lipsum.com Lorem ipsum http://www.lipsum.com/ adipiscing elit. http://www.lipsum.com/", 3)]
        [TestCase("http://www.lipsum.com http://www.lipsum.com http://www.lipsum.com http://www.lipsum.com", 4)]
        public void ExtractUris_ShouldExtractUrisFromMessage(string message, int nbrUrls)
        {
            var uris = _target.ExtractUris(message);
            Assert.AreEqual(nbrUrls, uris.Count());
        }

        // TODO: Add tests for character encoding problems
        // TODO: Add tests for titles that contain html-encoded strings
        // TODO: Don't use external resources for test
        [TestCase("http://www.google.com", "Google")]
        public void GetTitleFromUrl_ShouldExtractTitle(string url, string title)
        {
            var result = _target.GetTitleFromUrl(new Uri(url));
            Assert.True(result.StartsWith(title));
        }

        [SetUp]
        public void Init()
        {
            _target = new HttpHelper();
        }
    }
}