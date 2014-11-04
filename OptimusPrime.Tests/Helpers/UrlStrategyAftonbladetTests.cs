using NSubstitute;
using NUnit.Framework;
using OptimusPrime.Helpers;
using System;

namespace OptimusPrime.Tests.Helpers
{
    [TestFixture]
    public class UrlStrategyAftonbladetTests
    {
        private IHttpHelper _helper;
        private UrlStrategyAftonbladet _target;

        [SetUp]
        public void Init()
        {
            _helper = Substitute.For<IHttpHelper>();
            _target = new UrlStrategyAftonbladet(Arg.Any<Uri>(), _helper);
        }

        [TestCase("Stort bajsutsläpp i Trosa | Trosa | Aftonbladet", "Stort bajsutsläpp i Trosa")]
        public void RemovesRedundantTitlePart(string title, string expected)
        {
            _helper.GetTitleFromUrl(_target.Uri).Returns(title);
            Assert.AreEqual(expected, _target.ExtractInformationFromUrl());
        }
    }
}