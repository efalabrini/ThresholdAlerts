using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminAccess")]
public class ConfigInfoController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ConfigInfoController> _logger;
    private readonly TimedHostedService _timeHostedService;
    private readonly KeepAliveHostedService _keepAliveHostedService;

    private readonly ApplicationDbContext _dbContext;

    public ConfigInfoController(IWebHostEnvironment env,
     ILogger<ConfigInfoController> logger,
     TimedHostedService timeHostedService,
     ApplicationDbContext dbContext,
     KeepAliveHostedService keepAliveHostedService)
    {
        _env = env;
        _logger = logger;
        _timeHostedService = timeHostedService;
        _dbContext = dbContext;
        _keepAliveHostedService = keepAliveHostedService;
    }

    [HttpGet()]
    public ActionResult<List<string>> Get()
    {
        List<string> result = [];

        result.Add($"Environment: {_env.EnvironmentName}");

        var logLevel = _logger.IsEnabled(LogLevel.Trace) ? LogLevel.Trace :
                       _logger.IsEnabled(LogLevel.Debug) ? LogLevel.Debug :
                       _logger.IsEnabled(LogLevel.Information) ? LogLevel.Information :
                       _logger.IsEnabled(LogLevel.Warning) ? LogLevel.Warning :
                       _logger.IsEnabled(LogLevel.Error) ? LogLevel.Error :
                       LogLevel.Critical;

        result.Add($"ConfiguredLogLevel: {logLevel.ToString()}");

        result.Add("");

        var timeHostedServiceInfo = _timeHostedService.GetConfig();
        foreach( string thi in timeHostedServiceInfo)
        {
            result.Add(thi);
        }

        result.Add("");

        var keepAliveHostedServiceInfo = _keepAliveHostedService.GetConfig();
        foreach (string kahi in keepAliveHostedServiceInfo)
        {
            result.Add(kahi);
        }

        result.Add("");

        result.Add($"Database connection string: {_dbContext.Database.GetConnectionString()}");

        return result;
    }
}
