using System;
using System.Collections;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;

namespace AirQuality.Domain.Feed
{
    /// <summary>
    /// Represents a feed item.
    /// </summary>
    public class AirQuality
    {
        public string LastUpdate { get; private set; }
        public string ParticlePollution { get; set; }
        public string Ozone { get; private set; }
        public Tuple<string, string> Location { get; private set; }
        public string Agency { get; private set; }
        public int LocationIdentifier { get; private set; }
        public FeedType FeedType { get; set; }

        public override string ToString()
        {
            return $"{Location.Item1}, {Location.Item2}";
        }

        public static AirQuality CreateAirQuality(IDictionary<string, string> dict)
        { 
            AirQuality quality = null;
            if (!String.IsNullOrWhiteSpace(dict?["Location"]))
            {
                var location = dict["Location"].Split(',');
                quality = new AirQuality();
                quality.Location = location.Length == 2 ? Tuple.Create(location[0], location[1]) : Tuple.Create(location[0], "");
                quality.LastUpdate = dict["Last Update"];
                quality.ParticlePollution = dict["Particle Pollution"];
                quality.Ozone = dict["Ozone"];
                quality.Agency = dict["Agency"];
                quality.LocationIdentifier = int.Parse(dict["LocationIdentifier"]);
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
