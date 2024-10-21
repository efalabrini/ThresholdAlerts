using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeasurementController : ControllerBase
{
    private readonly IMeasurementService _measurementService;
    public MeasurementController(IMeasurementService measurementService)
    {
        _measurementService = measurementService;
        
    }

    [HttpGet]
    public ActionResult<List<MeasurementDto>> Get()
    {
        return _measurementService.GetAll();
    }

    [HttpPut]
    public ActionResult<Measurement> Put(PutMeasurementRequest request)
    {
        return _measurementService.PutMeasurement(request);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _measurementService.DeleteMeasurement(id);
        return NoContent();
    }

    [Authorize(Policy = "AdminAccess")]
    [HttpGet("[action]")]
    public ActionResult<List<MeasurementWithSubscriptionsDto>> GetWithSubscriptions()
    {
        return _measurementService.GetAllWithSubscriptions();
    }

    [Authorize(Policy = "AdminAccess")]
    [HttpPut("{id}/subscription")]
    public ActionResult<SubscriptionDto> PutSubscription([FromRoute] int id, [FromQuery] string email, [FromBody] PutSubscriptionRequest putSubscriptionRequest)
    {
        return SubscriptionDto.Create(_measurementService.PutSubscription(id,email,putSubscriptionRequest));
    }

    [Authorize(Policy = "AdminAccess")]
    [HttpDelete("{id}/subscription")]
    public ActionResult DeleteSubscription([FromRoute] int id, [FromQuery] string email)
    {

        _measurementService.DeleteSubscription(id,email);
        return NoContent();
    }
}
