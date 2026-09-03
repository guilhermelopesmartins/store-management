using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.DTOs;
using StoreManagement.Application.Services;

namespace StoreManagement.Api.Controllers;

[ApiController]
[Route("api/stores")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoresController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    private Guid CompanyId => (Guid)HttpContext.Items["CompanyId"]!;

    [HttpPost]
    public async Task<IActionResult> CreateStore(CreateStoreDto dto)
    {
        var created = await _storeService.CreateStoreAsync(CompanyId, dto);
        return CreatedAtAction(nameof(GetById), new { storeId = created.Id }, created);
    }

    [HttpGet("{storeId}")]
    public async Task<IActionResult> GetById(Guid storeId)
    {
        var store = await _storeService.GetByIdAsync(storeId, CompanyId);
        return store is null ? NotFound() : Ok(store);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stores = await _storeService.GetAllAsync(CompanyId);
        return Ok(stores);
    }

    [HttpPut("{storeId}")]
    public async Task<IActionResult> Update(Guid storeId, UpdateStoreDto dto)
    {
        var updated = await _storeService.UpdateAsync(storeId, CompanyId, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{storeId}")]
    public async Task<IActionResult> Delete(Guid storeId)
    {
        var deleted = await _storeService.DeleteAsync(storeId, CompanyId);
        return deleted ? NoContent() : NotFound();
    }
}