using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.Models.Requests;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMeasurementService _measurementService;

    [HttpGet("me/subscriptions")]
    public ActionResult<List<SubscriptionDto>> GetForCurrentUser()
    {
        var claims = HttpContext.User.Claims;
        var emailsClaim = claims.FirstOrDefault(c => c.Type == "emails")
                ?? throw new AppValidationException("Emails claim can't be null");
        string email = emailsClaim.Value;

        return _subscriptionService.ListByEmail(email);
    }

    public UserController(ISubscriptionService subscriptionService,
        IMeasurementService measurementService)
    {
        _subscriptionService = subscriptionService;
        _measurementService = measurementService;
    }

    [HttpPut("me/subscription")]
    public ActionResult<SubscriptionDto> PutSubscription([FromQuery] int measurementId,[FromBody] PutSubscriptionRequest putSubscriptionRequest)
    {
        var claims = HttpContext.User.Claims;
        var emailsClaim = claims.FirstOrDefault(c => c.Type == "emails")
                ?? throw new AppValidationException("Emails claim can't be null");
        string email = emailsClaim.Value;

        return SubscriptionDto.Create(_measurementService.PutSubscription(measurementId, email, putSubscriptionRequest));
    }

    [HttpDelete("me/subscription")]
    public ActionResult DeleteSubscription([FromQuery] int measurementId)
    {
        var claims = HttpContext.User.Claims;
        var emailsClaim = claims.FirstOrDefault(c => c.Type == "emails")
                ?? throw new AppValidationException("Emails claim can't be null");
        string email = emailsClaim.Value;

        _measurementService.DeleteSubscription(measurementId, email);
        return NoContent();
    }

}
