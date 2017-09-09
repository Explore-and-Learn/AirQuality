using AirQuality.Domain.Feed;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    public class AirQualityController : Controller
    {
        private readonly FeedParser _parser = new FeedParser();
       
        // GET api/values/5
        [HttpGet("{id}")]
        public AirQuality.Domain.Feed.AirQuality Get(int id)
        {
            return _parser.Parse(id, FeedType.Rss);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            //TODO
        }
    }
}
