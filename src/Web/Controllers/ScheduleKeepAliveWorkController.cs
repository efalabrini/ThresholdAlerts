using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAccess")]
    public class ScheduleKeepAliveWorkController : ControllerBase
    {
        private readonly KeepAliveHostedService _keepAliveHostedService;
        public ScheduleKeepAliveWorkController(KeepAliveHostedService keepAlivetimedHostedService)
        {
            _keepAliveHostedService = keepAlivetimedHostedService;
        }

        [HttpPost]
        public async Task<IActionResult> Start([FromQuery] int periodInMinutes = 2)
        {
            var hostUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            await _keepAliveHostedService.StartAsync(CancellationToken.None, periodInMinutes,hostUrl);
            return Ok();
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> Stop()
        {
            await _keepAliveHostedService.StopAsync(CancellationToken.None);
            return Ok();
        }

    }
}
