using Core.Exceptions;
using Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class TimedHostedService : IHostedService, IDisposable
{
    private readonly ILogger<TimedHostedService> _logger;
    private readonly AlertService _alertService;
    private Timer? _timer;
    private  bool _IsRunning = false;
    private int _periodInMinutes;
    private DateTime _startTime;

    private string _hostUrl = string.Empty;
    public TimedHostedService(ILogger<TimedHostedService> logger, AlertService alertService)
    {
        _logger = logger;
        _alertService = alertService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return StartAsync(CancellationToken.None, 1,"");
    }
    public Task StartAsync(CancellationToken cancellationToken,
     int periodInMinutes,
     string hostUrl)
    {
        if (_IsRunning)
            throw new AppValidationException("Work already schedule");

        _periodInMinutes = periodInMinutes;

        _logger.LogInformation("Starting timer to run every {periodInMinutes} minutes",_periodInMinutes);
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(_periodInMinutes));
        _IsRunning = true; 
        _startTime = DateTime.UtcNow;
        _hostUrl = hostUrl;
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        _logger.LogInformation("Work executed at {DateNow}. Next {NextSchedule}",DateTime.Now,DateTime.Now.AddMinutes(_periodInMinutes));
        _alertService.Alert();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_IsRunning)
            throw new AppValidationException("Work had already stopped.");

        _logger.LogInformation("Timed Hosted Service is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        _IsRunning = false ;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.LogInformation("TimedHostedService disposed at {DateTimeNow}",DateTime.Now);
        _timer?.Dispose();
    }

    public List<string> GetConfig()
    {
        List<string> result = [];
        result.Add($"TimedHostedService.HostUrl: {_hostUrl}");
        result.Add($"TimedHostedService.IsRunning: {_IsRunning}");
        result.Add($"TimedHostedService.StartTime: {_startTime}");
        result.Add($"TimedHostedService.PeriodInMinutes: {_periodInMinutes}");

        return result;
    }

    public AlertServiceStatusDto GetStatus()
    {
        return new AlertServiceStatusDto(_IsRunning,_periodInMinutes,_startTime);
    }
}