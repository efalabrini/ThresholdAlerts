using Core.Entities;

namespace Core.Interfaces
{
    public interface IMeasurementRepository
    {
        List<Measurement> List();

        List<Measurement> ListWithSubscriptions();
        Measurement? GetById(int id);

        Measurement? GetByName(string name);

        void UpdateAsync(Measurement entity);

        Measurement Add(Measurement entity);

        void Delete(Measurement entity);

        public int SaveChanges();
    
    }
}