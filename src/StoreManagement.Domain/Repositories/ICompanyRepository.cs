using StoreManagement.Domain.Entities;

namespace StoreManagement.Domain.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid companyId);
}