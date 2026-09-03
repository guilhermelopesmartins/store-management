using StoreManagement.Application.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Repositories;

namespace StoreManagement.Application.Services;

public sealed class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;

    public StoreService(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<StoreResponseDto> CreateStoreAsync(Guid companyId, CreateStoreDto dto)
    {
        var store = new Store
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = dto.Name,
            Address = dto.Address,
            Country = dto.Country,
            Timezone = dto.Timezone,
            IsActive = true
        };

        var created = await _storeRepository.AddAsync(store);

        return new StoreResponseDto
        {
            Id = created.Id,
            CompanyId = created.CompanyId,
            Name = created.Name,
            Address = created.Address,
            Country = created.Country,
            Timezone = created.Timezone,
            IsActive = created.IsActive
        };
    }
}