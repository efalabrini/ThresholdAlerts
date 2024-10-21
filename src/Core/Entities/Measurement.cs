using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class Measurement
{
    private readonly List<Subscription> _subscriptions = [];

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string FieldJsonPath { get; set; } = string.Empty;

    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions;


    public void AddSubscription(Subscription suscription)
    {
        _subscriptions.Add(suscription);
    }

    public void RemoveSubscription(Subscription suscription)
    {
        _subscriptions.Remove(suscription);
    }

    public Measurement(int id,string name, string unit, string apiUrl, string fieldJsonPath)
    {
        Id = id;
        Name = name;
        ApiUrl = apiUrl;
        Unit = unit;
        FieldJsonPath = fieldJsonPath;
    }

    private Measurement()
    {
        
    }

}
