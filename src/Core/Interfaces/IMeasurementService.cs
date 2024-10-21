using Core.Entities;
using Core.Models;
using Core.Models.Requests;

namespace Core.Interfaces;
public interface IMeasurementService
{
    List<MeasurementDto> GetAll();

    Measurement PutMeasurement(PutMeasurementRequest request);

    void DeleteMeasurement(int measurementId);

    List<MeasurementWithSubscriptionsDto> GetAllWithSubscriptions();
    Subscription PutSubscription(int measurementId, string email, PutSubscriptionRequest request);
    void DeleteSubscription(int measurementId, string email);

}