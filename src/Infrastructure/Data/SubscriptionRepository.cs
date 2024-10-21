using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class SubscriptionRepository : ISubscriptionRepository
{
    protected readonly ApplicationDbContext _context;

    public SubscriptionRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public List<Subscription> ListByEmail(string email)
    {
        return _context.Subscriptions
        .Include(x => x.Measurement)
        .Where(x => x.Email == email)
        .ToList();
    }

}
