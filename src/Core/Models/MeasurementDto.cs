using Core.Entities;
using Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models;

public record MeasurementDto(int Id,
string Name,
string Unit,
string ApiUrl,
string FieldJsonPath)
{
    public static MeasurementDto Create(Measurement entity)
    {
        var dto = new MeasurementDto(
            entity.Id,
            entity.Name,
            entity.Unit,
            entity.ApiUrl,
            entity.FieldJsonPath);

        return dto;
    }

    public static List<MeasurementDto> Create(IEnumerable<Measurement> entities)
    {
        var listDto = new List<MeasurementDto>();
        foreach (var entity in entities)
        {
            listDto.Add(Create(entity));
        }
        return listDto;
    }

}
