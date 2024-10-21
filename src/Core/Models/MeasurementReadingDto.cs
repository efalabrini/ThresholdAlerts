using Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models;

public  record MeasurementReadingDto(string Measurement,string Unit,Decimal Value, DateTime ReadAt,Decimal MinValue, DateTime MinValueReadAt, decimal MaxValue, DateTime MaxValueReadAt)
{
    public static MeasurementReadingDto Create(MeasurementReading entity)
    {
        var dto = new MeasurementReadingDto(
            entity.Measurement.Name,
            entity.Measurement.Unit,
            entity.Value,
            entity.ValueReadAt,
            entity.MinValue,
            entity.MinValueReadAt,
            entity.MaxValue,
            entity.MaxValueReadAt);

        return dto;


    }

    public static List<MeasurementReadingDto> Create(IEnumerable<MeasurementReading> entities)
    {
        var listDto = new List<MeasurementReadingDto>();
        foreach (var entity in entities)
        {
            listDto.Add(Create(entity));
        }
        return listDto;
    }
}


