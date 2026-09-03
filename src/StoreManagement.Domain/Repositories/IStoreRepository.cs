using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Repositories;

public interface IStoreRepository
{
    Task<Store> AddAsync(Store store);
    Task<Store?> GetByIdAsync(Guid storeId, Guid companyId);
    Task<IEnumerable<Store>> GetAllAsync(Guid companyId);
    Task UpdateAsync(Store store);
    Task DeleteAsync(Store store);
}