using backend.Models;

namespace backend.MetricsDAL.Interfaces;

public interface IPremiumMethodRepository
{
    Task<List<PremiumMethod>> GetAllAsync();
    Task AddAsync(PremiumMethod method);
}
