using CleanArchitecture.Domain.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ValueObjects;

public class MeasurementReading : ValueObject
{
    public Measurement Measurement { get; set; }

    public Decimal Value { get; set; }

    public DateTime ValueReadAt { get; set; }

    public Decimal MinValue { get; set; }

    public DateTime MinValueReadAt { get; set; }

    public Decimal MaxValue { get; set; }

    public DateTime MaxValueReadAt { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }

    public MeasurementReading(Measurement measurement, Decimal value, DateTime readAt, decimal minValue, DateTime minValueReadAt, decimal maxValue, DateTime maxValueReadAt)
    {
        Measurement = measurement;
        Value = value;
        ValueReadAt = readAt;
        MinValue = minValue;
        MinValueReadAt = minValueReadAt;
        MaxValue = maxValue;
        MaxValueReadAt = maxValueReadAt;
    }

}
