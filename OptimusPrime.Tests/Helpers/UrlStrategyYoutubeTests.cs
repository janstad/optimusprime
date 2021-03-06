﻿using NSubstitute;
using NUnit.Framework;
using OptimusPrime.Helpers;
using System;

namespace OptimusPrime.Tests.Helpers
{
    [TestFixture]
    public class UrlStrategyYoutubeTests
    {
        private IHttpHelper _helper;
        private UrlStrategyYoutube _target;

        [SetUp]
        public void Init()
        {
            _helper = Substitute.For<IHttpHelper>();
            _target = new UrlStrategyYoutube(Arg.Any<Uri>(), _helper);
        }

        [TestCase("Title - YouTube", "Title")]
        public void RemovesRedundantTitlePart(string title, string expected)
        {
            _helper.GetTitleFromUrl(_target.Uri).Returns(title);
            Assert.AreEqual(expected, _target.ExtractInformationFromUrl());
        }
    }
}