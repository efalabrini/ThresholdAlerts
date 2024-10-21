using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface ISubscriptionService
    {
        List<SubscriptionDto> ListByEmail(string email);
    }
}