using System;
using System.Collections.Generic;
using System.Text;

using StoreManagement.Application.DTOs;

namespace StoreManagement.Application.Services;

public interface IStoreService
{
    Task<StoreResponseDto> CreateStoreAsync(Guid companyId, CreateStoreDto dto);
    Task<StoreResponseDto?> GetByIdAsync(Guid storeId, Guid companyId);
}
