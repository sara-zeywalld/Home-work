using backend.MetricsDAL.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.MetricsDAL;

public class PremiumMethodRepository(AppDbContext db) : IPremiumMethodRepository
{
    public Task<List<PremiumMethod>> GetAllAsync() => db.PremiumMethods.Include(method => method.Metrics).ToListAsync();
    public async Task AddAsync(PremiumMethod method) { db.PremiumMethods.Add(method); await db.SaveChangesAsync(); }
}
