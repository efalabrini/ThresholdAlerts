using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ScheduleWorkController : ControllerBase
    {
        private readonly TimedHostedService _timeHostedService;
        public ScheduleWorkController(TimedHostedService timedHostedService)
        {
            _timeHostedService = timedHostedService;
        }

        [HttpPost]
        public async Task<IActionResult> Start([FromQuery] int periodInMinutes = 15)
        {
            var hostUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            await _timeHostedService.StartAsync(CancellationToken.None, periodInMinutes,hostUrl);
            return Ok();
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> Stop()
        {
            await _timeHostedService.StopAsync(CancellationToken.None);
            return Ok();
        }

    }
}
