using backend.MetricsDAL.Interfaces;
using backend.Models;

namespace backend.MetricsBL;

public class PremiumMethodService(IPremiumMethodRepository premium_method_repository) : Interfaces.IPremiumMethodService
{
    public async Task<IReadOnlyList<PremiumMethod>> GetAsync() => await premium_method_repository.GetAllAsync();

    public async Task<(bool IsValid, PremiumMethod Method)> CreateAsync(PremiumMethod method)
    {
        var isValid = !string.IsNullOrWhiteSpace(method.Number) && !string.IsNullOrWhiteSpace(method.Description);
        if (isValid) await premium_method_repository.AddAsync(method);
        return (isValid, method);
    }
}
