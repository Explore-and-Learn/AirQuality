using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirQuality
{
    /// <summary>
    /// Represents the XML format of a feed.
    /// </summary>
    public enum FeedType

    {
        /// <summary>
        /// Really Simple Syndication format.
        /// </summary>
        Rss,
        /// <summary>
        /// RDF site summary format.
        /// </summary>
        Rdf,
        /// <summary>
        /// Atom Syndication format.
        /// </summary>
        Atom

    }
}
