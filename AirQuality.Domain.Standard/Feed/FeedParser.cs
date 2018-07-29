using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using AirQuality.Domain.Feed;
using cloudscribe.HtmlAgilityPack;
#if WINDOWS_UWP
using HtmlAgilityPack;
#else

#endif

namespace AirQuality.Domain.Standard.Feed
{
    /// <summary>
    /// A simple RSS, RDF and ATOM feed parser.
    /// </summary>
    public class FeedParser
    {
        /// <summary>
        /// Parses the given <see cref="FeedType"/> and returns a <see cref="AirQuality"/>.
        /// </summary>
        /// <returns></returns>
        public async Task<AirQuality> Parse(int locationId, FeedType feedType)
        {
            var rawFeed = await GetFeedFromUrl($"http://feeds.enviroflash.info/rss/realtime/{locationId}.xml");
            switch (feedType)
            {
                case FeedType.Rss:
                    return ParseRss(rawFeed, locationId);
                case FeedType.Rdf:
                    return ParseRdf(rawFeed, locationId);
                case FeedType.Atom:
                    return ParseAtom(rawFeed, locationId);
                default:
                    throw new NotSupportedException($"{feedType.ToString()} is not supported");
            }
        }

        /// <summary>
        /// Parses an Atom feed and returns a <see cref="AirQuality"/>.
        /// </summary>
        public virtual AirQuality ParseAtom(string rawFeed, int locationIdentifier)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses an RSS feed and returns a <see cref="AirQuality"/>.
        /// </summary>
        public virtual AirQuality ParseRss(string rawFeed, int locationIdentifier)
        {
            try
            {
                XDocument doc = XDocument.Parse(rawFeed);
                if (doc.Root != null)
                {
                    var channel = doc.Root.Descendants().First(i => i.Name.LocalName == "channel");
                    // RSS/Channel/item
                    var entries = from item in channel.Elements().Where(i => i.Name.LocalName == "item")
                        select new
                        {
                            Content = item.Elements().First(i => i.Name.LocalName == "description").Value
                        };
                    return AirQuality.CreateAirQuality(GetAirQualityData(entries.FirstOrDefault()?.Content, locationIdentifier, FeedType.Rss));
                }
            }
            catch
            {}
            return new NullAirQuality();
        }

        /// <summary>
        /// Parses an RDF feed and returns a <see cref="AirQuality"/>/>.
        /// </summary>
        public virtual AirQuality ParseRdf(string rawFeed, int locationIdentifier)
        {
           throw new NotImplementedException();
        }

        private async Task<string> GetFeedFromUrl(string url)
        {
            var client = new HttpClient();
            var task = await client.GetAsync(url);
            var result = await task.Content.ReadAsStringAsync();
            return FixDescriptionHtmlIfExists(result);
        }

        private string FixDescriptionHtmlIfExists(string rawFeed)
        {
            var decodedFeed = System.Net.WebUtility.HtmlDecode(rawFeed);

            var startDescriptionElement = "<description>";
            var endDescriptionElement = "</description>";
            string update = decodedFeed;
            var firstDescriptionElementIndex = decodedFeed.IndexOf(startDescriptionElement, StringComparison.CurrentCultureIgnoreCase);
            var firstDescriptionClosingElementIndex = decodedFeed.IndexOf(endDescriptionElement, StringComparison.CurrentCultureIgnoreCase);
            //wrap html in <![CDATA[]]> to allow the xml parsers to ignore unpaired HTML tags
            if (firstDescriptionElementIndex != -1 && firstDescriptionClosingElementIndex != -1)
            {
                //looking for the second description element in string
                var nextStart = decodedFeed.IndexOf(startDescriptionElement, firstDescriptionElementIndex + 2, StringComparison.CurrentCultureIgnoreCase);
                var nextEnd = decodedFeed.IndexOf(endDescriptionElement, firstDescriptionClosingElementIndex + 2, StringComparison.CurrentCultureIgnoreCase);
                if (nextStart != -1 && nextEnd != -1)
                {

                    var content = decodedFeed.Substring(nextStart + startDescriptionElement.Length,
                        nextEnd - nextStart - startDescriptionElement.Length);
                    update = decodedFeed.Replace(content, $"<![CDATA[{content}]]>");
                }
            }
            return update;
        }

        private IDictionary<string, string> GetAirQualityData(string htmlContentAsString, int locationId, FeedType feedType)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContentAsString);
            var nodes = doc.DocumentNode.Descendants("div")
                .Select(d => d.InnerText.Replace("\n", "|").Replace("\t", ""));
            var values = new Dictionary<string, string>();
            values.Add("Last Update", "");
            values.Add("Location", "");
            values.Add("Agency", "");
            values.Add("Ozone", "");
            values.Add("Particle Pollution", "");
            values.Add("LocationIdentifier", locationId.ToString());
            values.Add("FeedType", feedType.ToString());
            foreach (var n in nodes)
            {
                var e = n.Replace("||", "|").Split('|');
                foreach (var i in e)
                {
                    if (!String.IsNullOrWhiteSpace(i))
                    {
                        var pair = values.FirstOrDefault(
                            kvp => i.Contains(kvp.Key) && String.IsNullOrWhiteSpace(kvp.Value));
                        if (pair.Key != default(string))
                        {
                            values[pair.Key] = RemoveLabel(i);
                        }
                    }
                }
            }
            return values;
        }

        private string RemoveLabel(string data)
        {
            var indexOfLabelDelimiter = data.IndexOf(':');
            return indexOfLabelDelimiter > -1 ? data.Substring(indexOfLabelDelimiter + 1).Trim() : data;
        }
    }

    internal static class StringExtensions
    {
        internal static string RemoveCdata(this string str)
        {
           return str.Replace("<![CDATA[", "").Replace("]]>", "");
        }
    }
}
