namespace Core.Models.Requests;

public record PutSubscriptionRequest(
    decimal? LowerThreshold,
    decimal? UpperThreshold
);