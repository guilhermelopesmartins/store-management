using StoreManagement.Application.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Exceptions;
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

        return ToDto(created);
    }

    public async Task<StoreResponseDto> GetByIdAsync(Guid storeId, Guid companyId)
    {
        var store = EnsureOwned(await _storeRepository.GetByIdReadOnlyAsync(storeId), storeId, companyId);

        return ToDto(store);
    }

    public async Task<IEnumerable<StoreResponseDto>> GetAllAsync(Guid companyId)
    {
        var stores = await _storeRepository.GetAllAsync(companyId);

        return stores.Select(ToDto);
    }

    public async Task<StoreResponseDto> UpdateAsync(Guid storeId, Guid companyId, UpdateStoreDto dto)
    {
        var store = await GetOwnedStoreAsync(storeId, companyId);

        store.Name = dto.Name;
        store.Address = dto.Address;
        store.Country = dto.Country;
        store.Timezone = dto.Timezone;
        store.IsActive = dto.IsActive;
        store.UpdatedAt = DateTime.UtcNow;

        await _storeRepository.UpdateAsync(store);

        return ToDto(store);
    }

    public async Task DeleteAsync(Guid storeId, Guid companyId)
    {
        var store = await GetOwnedStoreAsync(storeId, companyId);

        await _storeRepository.DeleteAsync(store);
    }

    private async Task<Store> GetOwnedStoreAsync(Guid storeId, Guid companyId)
    {
        var store = await _storeRepository.GetByIdAsync(storeId);

        return EnsureOwned(store, storeId, companyId);
    }

    private static Store EnsureOwned(Store? store, Guid storeId, Guid companyId)
    {
        if (store is null)
            throw new StoreNotFoundException(storeId);

        if (store.CompanyId != companyId)
            throw new StoreAccessDeniedException(storeId, companyId);

        return store;
    }

    private static StoreResponseDto ToDto(Store store) => new()
    {
        Id = store.Id,
        CompanyId = store.CompanyId,
        Name = store.Name,
        Address = store.Address,
        Country = store.Country,
        Timezone = store.Timezone,
        IsActive = store.IsActive
    };
}
