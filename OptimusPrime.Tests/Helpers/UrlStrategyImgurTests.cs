using NSubstitute;
using NUnit.Framework;
using OptimusPrime.Helpers;
using System;

namespace OptimusPrime.Tests.Helpers
{
    [TestFixture]
    public class UrlStrategyImgurTests
    {
        private IHttpHelper _helper;
        private UrlStrategyImgur _target;

        [SetUp]
        public void SetUp()
        {
            _helper = Substitute.For<IHttpHelper>();
        }

        [TestCase("http://i.imgur.com/Ik6wjKr.jpg", "http://imgur.com/Ik6wjKr")]
        [TestCase("http://i.imgur.com/Ik6wjKr.jpeg", "http://imgur.com/Ik6wjKr")]
        [TestCase("http://i.imgur.com/Ik6wjKr.gif", "http://imgur.com/Ik6wjKr")]
        [TestCase("http://i.imgur.com/Ik6wjKr.png", "http://imgur.com/Ik6wjKr")]
        public void ShouldPrepareUriWhenInstanceCreated(string input, string expected)
        {
            _target = new UrlStrategyImgur(new Uri(input), _helper);
            Assert.AreEqual(expected, _target.Uri.AbsoluteUri);
        }
    }
}