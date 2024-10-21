using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class Subscription
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public Measurement Measurement { get; set; } 

    public decimal? LowerThreshold { get; set; }

    public decimal? UpperThreshold { get; set; }

    public Subscription(int id,string email, Measurement measurement, decimal? lowerThreshold, decimal? upperThreshold)
    {
        Id = id;
        Email = email;
        Measurement = measurement;
        LowerThreshold = lowerThreshold;
        UpperThreshold = upperThreshold;
    }

    #pragma warning disable CS8618
    private Subscription()
    {
        
    }
    #pragma warning restore CS8618

}
