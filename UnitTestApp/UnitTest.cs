
using System.Linq;
using AirQuality;
using AirQuality.Feed;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestApp
{
    [TestClass]
    public class FeedParserTest
    {
        [TestMethod]
        public void TestRssParser()
        {

            var url = "http://feeds.enviroflash.info/rss/forecast/116.xml";
            var parser = new FeedParser();
            var items = parser.Parse(url, FeedType.Rss);
            Assert.IsTrue(items.Count > 0);
        }
    }
}
