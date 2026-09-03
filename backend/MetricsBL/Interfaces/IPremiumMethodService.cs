using backend.Models;

namespace backend.MetricsBL.Interfaces;

public interface IPremiumMethodService
{
    Task<IReadOnlyList<PremiumMethod>> GetAsync();
    Task<(bool IsValid, PremiumMethod Method)> CreateAsync(PremiumMethod method);
}
