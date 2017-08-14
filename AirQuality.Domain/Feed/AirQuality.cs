using System;
using System.Collections;
using System.Collections.Generic;

namespace AirQuality.Domain.Feed
{
    /// <summary>
    /// Represents a feed item.
    /// </summary>
    public class AirQuality
    {
        public string LastUpdate { get; }
        public string ParticlePollution { get; set; }
        public string Ozone { get; }
        public string Location { get; }
        public string Agency { get; }
        public int LocationIdentifier { get; }
        public FeedType FeedType { get; set; }

        public AirQuality(IDictionary<string, string> dict)
        {
            if (dict != null)
            {
                Location = dict["Location"];
                LastUpdate = dict["Last Update"];
                ParticlePollution = dict["Particle Pollution"];
                Ozone = dict["Ozone"];
                Agency = dict["Agency"];
                LocationIdentifier = int.Parse(dict["LocationIdentifier"]);
            }

        }
    }
}
