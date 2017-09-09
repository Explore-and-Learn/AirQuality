using System.Collections.Generic;
using System.Text.RegularExpressions;
using AirQuality.Domain.Feed;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirQuality.UnitTests
{
    [TestClass]
    public class FeedParserTests
    {
        [TestMethod]
        public void TestRssParser()
        {
           var validFeeds = new Dictionary<int, Domain.Feed.AirQuality>();

            for (int j = 1; j < 900; j++)
            {
                var parser = new FeedParser();
                var airQuality = parser.Parse(j, FeedType.Rss);
                if (airQuality is NullAirQuality) continue;
                Assert.IsNotNull(airQuality.Location,
                    $"Expected a valid location for {j} but nothing was returned.");
                validFeeds.Add(j, airQuality);
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



    public class TestAirQuality : Domain.Feed.AirQuality
    {
        public TestAirQuality()
        {
            ParticlePollution = "Good  - 12 AQI - Particle Pollution (2.5 microns)";
        }
    }
}
