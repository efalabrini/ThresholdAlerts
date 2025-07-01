using Core.Models;
using Core.Services;
using Core.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlertController : ControllerBase
{
    private readonly AlertService _alertService;
    public AlertController(AlertService alertService)
    {
        _alertService = alertService;
    }

    /// <summary>
    /// For each measure resgistered in the app, fetches the value.
    /// Then, for each suscription registered in the measurement, if exist, evaluates the thresholds
    /// and creates a message if any of the threshold is surpased. If there is a message to send, notifies the suscriber.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "admin")]
    [HttpPost]
    public IActionResult Alert()
    {
        _alertService.Alert();
        return Ok();
    }


    [HttpGet("[Action]")]
    public ActionResult<List<MeasurementReadingDto>> ListMeasurementReadings()
    {
        return _alertService.ListMeasurementReadings().ToList();
    }

    [HttpGet("[Action]")]
    public ActionResult<int> GetReadingsCount()
    {
        return _alertService.ReadingsCount;
    }
}