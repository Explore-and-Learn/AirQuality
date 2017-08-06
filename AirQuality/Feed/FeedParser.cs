using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Xml.Linq;
using Windows.Data.Html;
using Windows.Security.Authentication.Web.Provider;

namespace AirQuality.Feed
{
    /// <summary>
    /// A simple RSS, RDF and ATOM feed parser.
    /// </summary>
    public class FeedParser
    {
        /// <summary>
        /// Parses the given <see cref="FeedType"/> and returns a <see cref="IList&amp;lt;Item&amp;gt;"/>.
        /// </summary>
        /// <returns></returns>
        public IList<Item> Parse(string url, FeedType feedType)
        {
            var rawFeed = GetFeedFromUrl(url);
            switch (feedType)
            {
                case FeedType.Rss:
                    return ParseRss(rawFeed);
                case FeedType.Rdf:
                    return ParseRdf(rawFeed);
                case FeedType.Atom:
                    return ParseAtom(rawFeed);
                default:
                    throw new NotSupportedException(string.Format("{0} is not supported", feedType.ToString()));
            }
        }

        /// <summary>
        /// Parses an Atom feed and returns a <see cref="IList&amp;lt;Item&amp;gt;"/>.
        /// </summary>
        public virtual IList<Item> ParseAtom(string rawFeed)
        {
            try
            {
                XDocument doc = XDocument.Parse(rawFeed);
                // Feed/Entry
                var entries = from item in doc.Root.Elements().Where(i => i.Name.LocalName == "entry")
                    select new Item
                    {
                        FeedType = FeedType.Atom,
                        Content = item.Elements().First(i => i.Name.LocalName == "content").Value,
                        Link = item.Elements().First(i => i.Name.LocalName == "link").Attribute("href").Value,
                        PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "published").Value),
                        Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                    };
                return entries.ToList();
            }
            catch
            {
                return new List<Item>();
            }
        }

        /// <summary>
        /// Parses an RSS feed and returns a <see cref="IList&amp;lt;Item&amp;gt;"/>.
        /// </summary>
        public virtual IList<Item> ParseRss(string rawFeed)
        {
            XDocument doc = XDocument.Parse(rawFeed);
            var channel = doc.Root.Descendants().First(i => i.Name.LocalName == "channel");
            // RSS/Channel/item
            var entries = from item in channel.Elements().Where(i => i.Name.LocalName == "item")
                select new Item
                
                {
                    FeedType = FeedType.Rss,
                    Content = item.Elements().First(i => i.Name.LocalName == "description").Value,
                    Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                    PublishDate = ParseDate(channel.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                    Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                };
            return entries.ToList();
        }

        /// <summary>
        /// Parses an RDF feed and returns a <see cref="IList&amp;lt;Item&amp;gt;"/>.
        /// </summary>
        public virtual IList<Item> ParseRdf(string rawFeed)
        {
            try
            {
                XDocument doc = XDocument.Parse(rawFeed);
                // <item> is under the root
                var entries = from item in doc.Root.Descendants().Where(i => i.Name.LocalName == "item")
                    select new Item
                    {
                        FeedType = FeedType.Rdf,
                        Content = item.Elements().First(i => i.Name.LocalName == "description").Value.RemoveCdata(),
                        Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                        PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "date").Value),
                        Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                    };
                return entries.ToList();
            }
            catch
            {
                return new List<Item>();
            }
        }

        private DateTime ParseDate(string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }

        private string GetFeedFromUrl(string url)
        {
            var client = new HttpClient();
            var task = client.GetAsync(url);
            var raw = task.Result.Content.ReadAsStringAsync().Result;
            return FixDescriptionHtmlIfExists(raw);
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
    }

    internal static class StringExtensions
    {
        internal static string RemoveCdata(this string str)
        {
           return str.Replace("<![CDATA[", "").Replace("]]>", "");
        }
    }
}
