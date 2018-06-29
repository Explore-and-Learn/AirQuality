using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AirQuality.Domain.Feed;

namespace AirQuality.Domain.Standard.Feed
{
    /// <summary>
    /// Represents a feed item.
    /// </summary>
    public class AirQuality
    {
        public string LastUpdate { get; private set; }
        public string ParticlePollution { get; set; }
        public string Ozone { get; private set; }
        public ValueTuple<string,string> Location { get; private set; }
        public string Agency { get; private set; }
        public int LocationIdentifier { get; private set; }
        public FeedType FeedType { get; set; }

        public override string ToString()
        {
            return $"{Location.Item1}, {Location.Item2}";
        }

        /// <summary>
        /// Helper method that returns an <see cref="AirQuality"/> instance based on values stored in a dictionary
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static AirQuality CreateAirQuality(IDictionary<string, string> dict)
        { 
            AirQuality quality = null;
            if (!String.IsNullOrWhiteSpace(dict?["Location"]))
            {
                var location = dict["Location"].Split(',');
                quality = new AirQuality
                {
                    Location = location.Length == 2
                        ? ValueTuple.Create(location[0], location[1])
                        : ValueTuple.Create(location[0], ""),
                    LastUpdate = dict["Last Update"],
                    ParticlePollution = dict["Particle Pollution"],
                    Ozone = dict["Ozone"],
                    Agency = dict["Agency"],
                    LocationIdentifier = int.Parse(dict["LocationIdentifier"])
                };
            }
            else
            {
                quality = new NullAirQuality();
            }
            return quality;
        }
    }
    public class NullAirQuality : AirQuality
    {
        public override string ToString()
        {
            return "Empty";
        }
    }
}
