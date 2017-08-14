
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AirQuality.Domain.Feed;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestApp
{
    [TestClass]
    public class FeedParserTest
    {
        [TestMethod]
        public void TestRssParser()
        {
            for (int j = 1; j < 200; j++)
            {
                var parser = new FeedParser();
                var airQuality = parser.Parse(j, FeedType.Rss);
                Assert.IsTrue(!String.IsNullOrWhiteSpace(airQuality.ParticlePollution),
                    $"Expected a valid location for {j} but nothing was returned.");

            }
        }

        [TestMethod]
        public void TestLocationRegex()
        {
            var airQuality = new TestAirQuality();
            var match = Regex.Match(airQuality.ParticlePollution, @"^.*- (\d+)");
            airQuality.ParticlePollution = match.Value.Substring(match.Value.IndexOf('-') + 1).Trim();


            Assert.IsTrue(match.Value == "Good  - 12",
                $"Expected ParticlePollution match of 'Good - 12' but received {match.Value}");
            Assert.IsTrue(airQuality.ParticlePollution == "12",
                $"Expected final ParticlePollution value of '12' but received {airQuality.ParticlePollution}");

        }
    }



    public class TestAirQuality : AirQuality.Domain.Feed.AirQuality
    {
        public TestAirQuality() : base(null)
        {
            ParticlePollution = "Good  - 12 AQI - Particle Pollution (2.5 microns)";
        }
        public TestAirQuality(IDictionary<string, string> dict) : base(dict)
        { }
    }
}
