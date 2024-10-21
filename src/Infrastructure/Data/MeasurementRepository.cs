using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Infrastructure.Data;

public class MeasurementRepository : IMeasurementRepository
{
    protected readonly ApplicationDbContext _context;

    public MeasurementRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public List<Measurement> List()
    {
        return _context.Measurements
        .ToList();
    }

    public List<Measurement> ListWithSubscriptions()
    {
        return _context.Measurements
            .Include(x => x.Subscriptions)
        .ToList();
    }

    public Measurement Add(Measurement entity)
    {
        _context.Measurements.Add(entity);

        _context.SaveChanges();

        return entity;
    }


    public void UpdateAsync(Measurement entity)
    {
        _context.Measurements.Update(entity);
        _context.SaveChanges();
    }

    public Measurement? GetById(int id)
    {
        return _context.Measurements
        .Include(x => x.Subscriptions)
        .FirstOrDefault(x => x.Id == id);
    }

    public Measurement? GetByName(string name)
    {
        return _context.Measurements
        .FirstOrDefault(x => x.Name == name);
    }

    public void Delete(Measurement entity)
    {
        _context.Measurements.Remove(entity);

        _context.SaveChanges();
    }

     public int SaveChanges()
    {
        return _context.SaveChanges();
    }

}
