using StoreManagement.Application.DTOs;
using StoreManagement.Domain.Repositories;

namespace StoreManagement.Application.Services;

public sealed class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;

    public StoreService(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public Task<StoreResponseDto> CreateStoreAsync(Guid companyId, CreateStoreDto dto)
    {
        throw new NotImplementedException();
    }
}