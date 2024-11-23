using Core.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Core.Extensions;
using System.Xml.Linq;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Core.ValueObjects;
using System.Runtime.CompilerServices;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public class AlertService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AlertService> _logger;
    private readonly List<MeasurementReading> _measurementReadings = [];
    

    public AlertService(IServiceProvider serviceProvider,
     INotificationService notificationService,
      ILogger<AlertService> logger)
    {
        _serviceProvider = serviceProvider;
        _notificationService = notificationService;
        _logger = logger;
        ReadingsCount = 0;
    }

    private void UpdateMeasurementReadings(Measurement measurement, Decimal value, DateTime readAt)
    {
        MeasurementReading? measurementReading = _measurementReadings.FirstOrDefault(x => x.Measurement.Id == measurement.Id);

        if (measurementReading == null)
        {
            measurementReading = new MeasurementReading(measurement, value, readAt, value,readAt, value,readAt);
            _measurementReadings.Add(measurementReading);
        }
        else
        {
            measurementReading.Value = value;
            measurementReading.ValueReadAt = readAt;
            if (value < measurementReading.MinValue)
            {
                measurementReading.MinValue = value;
                measurementReading.MinValueReadAt = readAt;
            }

            if (value > measurementReading.MaxValue)
            {
                measurementReading.MaxValue = value;
                measurementReading.MaxValueReadAt = readAt;
            }
        }

    }

    public async void Alert()
    {
        var measurements = new List<Measurement>();
        using (var scope = _serviceProvider.CreateScope())
        {
            var measurementRepository = scope.ServiceProvider.GetRequiredService<IMeasurementRepository>();
            measurements = measurementRepository.ListWithSubscriptions();
        }
         
        decimal value;

        foreach (var measurement in measurements)
        {
            try
            {
                _logger.LogInformation("{measurement.Name} fetch starting",measurement.Name);
                value = await GetValueAsync(measurement);
                _logger.LogInformation("{measurement.Name} fetch finished. Value: {value}", measurement.Name, value);
                UpdateMeasurementReadings(measurement, value,DateTime.UtcNow);

                string msg = string.Empty;

                foreach (var suscription in measurement.Subscriptions)
                {
                    _logger.LogInformation("Evaluating subscription: {suscription.Email}, LowerThreshold: {suscription.LowerThreshold}, UpperThreshold: {suscription.UpperThreshold}",
                        suscription.Email, suscription.LowerThreshold, suscription.UpperThreshold);
                    if (suscription.LowerThreshold != null)
                    {
                        if (value < suscription.LowerThreshold)
                        {
                            msg += $"{measurement.Name} is under {suscription.LowerThreshold} {measurement.Unit} ({value}).";
                        }
                    }

                    if (suscription.UpperThreshold != null)
                    {
                        if (value > suscription.UpperThreshold)
                        {
                            msg += $"{measurement.Name} is above {suscription.UpperThreshold} {measurement.Unit} ({value}).";
                        }
                    }

                    if (msg != string.Empty)
                    {
                        await _notificationService.NotifyAsync(msg, suscription.Email);
                    }

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        ReadingsCount++;
    }

    private async Task<decimal> GetValueAsync(Measurement measurement)
    {

        HttpClient httpClient = new();
        using HttpResponseMessage response = await httpClient.GetAsync(measurement.ApiUrl);

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        JsonNode data = JsonNode.Parse(jsonResponse)!;
        JsonNode field = data.GetValue(measurement.FieldJsonPath);

        var aux = field.AsValue().ToString();

        var numberFormat = new NumberFormatInfo();
        numberFormat.PercentDecimalSeparator = ".";
        numberFormat.CurrencyDecimalSeparator = ".";
        numberFormat.NumberDecimalSeparator = ".";
        numberFormat.NumberGroupSeparator = "";


        decimal.TryParse(aux, numberFormat, out decimal result);

        return result;
    }


    public void TestLog()
    {
        _logger.LogInformation("Information log");
        _logger.LogError("Error log");
        _logger.LogWarning("Warning log");
    }

    public IReadOnlyList<MeasurementReadingDto> ListMeasurementReadings()
    {
        return MeasurementReadingDto.Create(_measurementReadings);
    }

    /// <summary>
    /// Count of iterations comprising evaluating every registered measure and notify if necessary.
    /// </summary>
    public int ReadingsCount { get; private set; }

}
