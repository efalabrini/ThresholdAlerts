using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models;

public record SubscriptionDto( int Id,
    string Email,
    string Measurement,
    string MeasurementUnit,
    decimal? LowerThreshold,
    decimal? UpperThreshold,
    int MeasurementId)
{
    public static SubscriptionDto Create(Subscription entity)
    {
        var dto = new SubscriptionDto(
            entity.Id,
            entity.Email,
            entity.Measurement.Name,
            entity.Measurement.Unit,
            entity.LowerThreshold,
            entity.UpperThreshold,
            entity.Measurement.Id);

        return dto;
    }

    public static List<SubscriptionDto> Create(IEnumerable<Subscription> entities)
    {
        var listDto = new List<SubscriptionDto>();
        foreach (var entity in entities)
        {
            listDto.Add(Create(entity));
        }
        return listDto;
    }
}
