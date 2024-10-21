using System.Diagnostics.Metrics;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.Models.Requests;

namespace Core.Services;

public class MeasurementService : IMeasurementService
{
    private readonly IMeasurementRepository _measurementRepository;

    public MeasurementService(IMeasurementRepository measurementRepository)
    {
        _measurementRepository = measurementRepository;
    }

    public List<MeasurementDto> GetAll()
    {
        return MeasurementDto.Create(_measurementRepository.List());
    }

    public List<MeasurementWithSubscriptionsDto> GetAllWithSubscriptions()
    {
        return MeasurementWithSubscriptionsDto.Create(_measurementRepository.ListWithSubscriptions());
    }

    public void DeleteSubscription(int measurementId, string email)
    {
        Measurement? m = _measurementRepository.GetById(measurementId) 
            ?? throw new NotFoundException(nameof(Measurement),measurementId);

        Subscription? s = m.Subscriptions.FirstOrDefault(s => s.Email == email);

        if (s == null)
        {
            throw new NotFoundException(nameof(Subscription),email);
        }
        else
        {
            m.RemoveSubscription(s);
            _measurementRepository.SaveChanges();
        }

    }

    public Subscription PutSubscription(int measurementId, string email, PutSubscriptionRequest request)
    {
        Measurement? m = _measurementRepository.GetById(measurementId) 
            ?? throw new NotFoundException(nameof(Measurement),measurementId);
        
        Subscription? s = m.Subscriptions.FirstOrDefault(s => s.Email == email);

        if (s == null)
        {
            Subscription newSubscription = new Subscription(0,
            email,
            m,
            request.LowerThreshold,
            request.UpperThreshold);

            m.AddSubscription(newSubscription);
            _measurementRepository.SaveChanges();

            return newSubscription;
        }
        else
        {
            s.Email = email;
            s.LowerThreshold = request.LowerThreshold;
            s.UpperThreshold = request.UpperThreshold;

            _measurementRepository.SaveChanges();
            return s;

        }
    }

    public Measurement PutMeasurement(PutMeasurementRequest request)
    {

        var m = _measurementRepository.GetByName(request.Name);

        if (m == null)
        {
            m = new Measurement(0,
                request.Name,
                request.Unit,
                request.ApiUrl,
                request.FieldJsonPath);

            _measurementRepository.Add(m);
        }
        else
        {
            m.Name = request.Name;
            m.Unit = request.Unit;
            m.ApiUrl = request.ApiUrl;
            m.FieldJsonPath = request.FieldJsonPath;
            _measurementRepository.SaveChanges();
        }

        return m;

    }

    public void DeleteMeasurement(int  measurementId)
    {
        var m = _measurementRepository.GetById(measurementId)
            ?? throw new NotFoundException(nameof(Measurement), measurementId);

        _measurementRepository.Delete(m);
    }


}