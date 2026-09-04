using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Repositories;
using StoreManagement.Infrastructure.Persistence;

namespace StoreManagement.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly AppDbContext _context;

    public StoreRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Store> AddAsync(Store store)
    {
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
        return store;
    }

    public async Task<Store?> GetByIdAsync(Guid storeId)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId);
    }

    public async Task<Store?> GetByIdReadOnlyAsync(Guid storeId)
    {
        return await _context.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == storeId);
    }

    public async Task<IEnumerable<Store>> GetAllAsync(Guid companyId)
    {
        return await _context.Stores
            .AsNoTracking()
            .Where(s => s.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Store store)
    {
        _context.Stores.Update(store);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Store store)
    {
        _context.Stores.Remove(store);
        await _context.SaveChangesAsync();
    }
}