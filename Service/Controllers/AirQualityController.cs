using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AirQuality.Domain.Feed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    public class AirQualityController : Controller
    {
        private readonly FeedParser _parser = new FeedParser();
        private readonly IConfiguration _configuration;

        public AirQualityController(IConfiguration config)
        {
            _configuration = config;
        }

        // GET api/AirQuality/116 - return Portland, OR
        [HttpGet("{id}")]
        public AirQuality.Domain.Feed.AirQuality Get(int id)
        {
            return _parser.Parse(id, FeedType.Rss);
        }

        /// <summary>
        /// This refreshes the data in azure storage
        /// </summary>
        [HttpGet("UpdateData")]
        public void UpdateData()
        {
            var validFeeds = GetValidFeeds();
            foreach (var kvp in validFeeds)
            {
                SaveToAirQualityRepository(kvp.Value);
            }
        }

        public async void SaveToAirQualityRepository(AirQuality.Domain.Feed.AirQuality airQuality)
        {
            try
            {
                string connStr = GetConnectionString();
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);
                CloudTableClient client = storageAccount.CreateCloudTableClient();
                CloudTable table = client.GetTableReference("AirQuality");
                await table.CreateIfNotExistsAsync();
                var entity = new AirQualityEntity(airQuality);
                TableOperation insertOp = TableOperation.InsertOrReplace(entity);
                var result = await table.ExecuteAsync(insertOp);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Get dictionary of air quality feeds
        /// </summary>
        /// <param name="id"> largest id to retrieve. Currently 900</param>
        /// <returns></returns>
        private static Dictionary<int, AirQuality.Domain.Feed.AirQuality> GetValidFeeds(int id = 900)
        {
            var validFeeds = new Dictionary<int, AirQuality.Domain.Feed.AirQuality>();
            for (int j = 1; j < id; j++)
            {
                var parser = new FeedParser();
                var airQuality = parser.Parse(j, FeedType.Rss);
                if (airQuality is NullAirQuality) continue;
                validFeeds.Add(j, airQuality);
            }
            return validFeeds;
        }

        public string GetConnectionString()
        {
            return ConfigurationExtensions
                .GetConnectionString(_configuration, "DefaultConnection");
        }
    }

    public class AirQualityEntity : TableEntity
    {
        public AirQualityEntity(AirQuality.Domain.Feed.AirQuality airQuality)
        {
            this.RowKey = airQuality.LocationIdentifier.ToString();
            this.Timestamp = DateTimeOffset.Now;
            this.ETag = "*";
            this.PartitionKey = "Primary";
            this.Agency = airQuality.Agency;
            this.LastUpdate = airQuality.LastUpdate;
            this.Ozone = airQuality.Ozone;
            this.ParticlePollution = airQuality.ParticlePollution;
            this.City = airQuality.Location.Item1;
            this.State = airQuality.Location.Item2;
        }

        public string ParticlePollution { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Agency { get; set; }
        public string LastUpdate { get; set; }
        public string Ozone { get; set; }
    }
}
