using System.Threading.Tasks;
using AirQuality.Domain.Feed;
using AirQuality.Domain.Standard.Feed;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("v1/[controller]")]
    public class AirQualityController : Controller
    {
        private const string EmptyResults = "Empty";
        private readonly FeedParser _parser = new FeedParser();
       
        // GET api/AirQuality/116 - return Portland, OR
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var results = await _parser.Parse(id, FeedType.Rss);
            return results.ToString() != EmptyResults
                ? (IActionResult) Ok(results)
                : NotFound($"No results returned for {id}");
        }
    }
}
