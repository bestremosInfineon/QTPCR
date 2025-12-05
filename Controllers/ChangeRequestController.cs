using Microsoft.AspNetCore.Mvc;
using QTPCR.Services.Contracts;

namespace QTPCR.Controllers
{
    [ApiController]
    public class ChangeRequestController : Controller
    {
        private readonly IChangeRequestServices _changeRequestServices;
        private readonly ILogsServices _logsServices;

        public ChangeRequestController(ILogsServices logsServices, IChangeRequestServices changeRequestServices)
        {
            _logsServices = logsServices;
            _changeRequestServices = changeRequestServices;
        }

        [HttpPost]
        [Route("getStates")]
        public async Task<IActionResult> GetStates(string qtpNumber)
        {
            try
            {
                
                return Ok(await _changeRequestServices.GetRealisAllTestState(qtpNumber));
            }
            catch (Exception err)
            {
                return BadRequest($"{err.Message}");
            }
        }
    }
}
