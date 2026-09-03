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

    public async Task<StoreResponseDto?> GetByIdAsync(Guid storeId, Guid companyId)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, companyId);

        if (store is null)
            return null;

        return new StoreResponseDto
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

    public async Task<IEnumerable<StoreResponseDto>> GetAllAsync(Guid companyId)
    {
        var stores = await _storeRepository.GetAllAsync(companyId);

        return stores.Select(store => new StoreResponseDto
        {
            Id = store.Id,
            CompanyId = store.CompanyId,
            Name = store.Name,
            Address = store.Address,
            Country = store.Country,
            Timezone = store.Timezone,
            IsActive = store.IsActive
        });
    }

    public async Task<StoreResponseDto?> UpdateAsync(Guid storeId, Guid companyId, UpdateStoreDto dto)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, companyId);

        if (store is null)
            return null;

        store.Name = dto.Name;
        store.Address = dto.Address;
        store.Country = dto.Country;
        store.Timezone = dto.Timezone;
        store.IsActive = dto.IsActive;
        store.UpdatedAt = DateTime.UtcNow;

        await _storeRepository.UpdateAsync(store);

        return new StoreResponseDto
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

    public async Task<bool> DeleteAsync(Guid storeId, Guid companyId)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, companyId);

        if (store is null)
            return false;

        await _storeRepository.DeleteAsync(store);

        return true;
    }
}