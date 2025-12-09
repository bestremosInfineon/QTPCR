using Microsoft.AspNetCore.Mvc;
using QTPCR.Services.Contracts;

namespace QTPCR.Controllers
{
    [ApiController]
    public class ChangeRequestController : Controller
    {
        private readonly IChangeRequestServices _changeRequestServices;
        private readonly ILogsServices _logsServices;
        private readonly ITokenServices _tokenServices;

        public ChangeRequestController(ILogsServices logsServices, IChangeRequestServices changeRequestServices, ITokenServices tokenServices)
        {
            _logsServices = logsServices;
            _changeRequestServices = changeRequestServices;
            _tokenServices = tokenServices;
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

        [HttpGet]
        [Route("getToken")]
        public async Task<IActionResult> GetToken()
        {
            try
            {
                return Ok(await _tokenServices.GetTokenResponse("asdasdasd"));
            }
            catch (Exception err)
            {
                return BadRequest($"{err.Message}");
            }
        }
    }
}
