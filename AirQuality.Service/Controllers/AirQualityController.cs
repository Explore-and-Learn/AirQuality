using System.Collections.Generic;
using System.Web.Http;

namespace AirQuality.Service.Controllers
{
    
    /// <summary>
    /// This controller returns the air quality reading and or reading for specific US locations
    /// </summary>
    public class AirQualityController : ApiController
    {
       
        [HttpGet, Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(id * id);
        }
    }
}
