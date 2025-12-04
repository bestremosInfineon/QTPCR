using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace QTPCR.Controllers
{
    [RoutePrefix("changerequest")]
    public class ChangeRequestController : ApiController
    {
        [HttpPost]
        [Route("getStates")]
        public async Task<IHttpActionResult> GetStates()
        {
            try
            {
                return Ok();
            }
            catch (Exception err)
            {
                return BadRequest($"{err.Message}");
            }
        }
    }
}
