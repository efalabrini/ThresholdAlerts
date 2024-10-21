using System.Diagnostics.Metrics;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{

    public DbSet<Measurement> Measurements { get; set; }

    public DbSet<Subscription> Subscriptions{ get; set; }

     public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
     : base(options) 
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

    }    

}