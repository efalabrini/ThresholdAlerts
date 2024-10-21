using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models;

public record MeasurementWithSubscriptionsDto(int Id,
string Name,
string Unit,
string ApiUrl,
List<SubscriptionDto> Subscriptions)
{
    public static MeasurementWithSubscriptionsDto Create(Measurement entity)
    {
        var dto = new MeasurementWithSubscriptionsDto(
            entity.Id,
            entity.Name,
            entity.Unit,
            entity.ApiUrl,
            SubscriptionDto.Create(entity.Subscriptions));

        return dto;
    }

    public static List<MeasurementWithSubscriptionsDto> Create(IEnumerable<Measurement> entities)
    {
        var listDto = new List<MeasurementWithSubscriptionsDto>();
        foreach (var entity in entities)
        {
            listDto.Add(Create(entity));
        }
        return listDto;
    }

}
