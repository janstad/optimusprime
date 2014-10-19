using NUnit.Framework;
using OptimusPrime.Helpers;
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

        [SetUp]
        public void Init()
        {
            _target = new HttpHelper();
        }
    }
}