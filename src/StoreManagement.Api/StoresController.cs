using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.DTOs;
using StoreManagement.Application.Services;
using System.Security.Claims;

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

    [HttpPost]
    public async Task<IActionResult> CreateStore(CreateStoreDto dto)
    {
        var companyId = Guid.Parse(User.FindFirstValue("companyId")!);

        var created = await _storeService.CreateStoreAsync(companyId, dto);

        return CreatedAtAction(
            nameof(GetById),
            new { storeId = created.Id },
            created);
    }

    [HttpGet("{storeId}")]
    public async Task<IActionResult> GetById(Guid storeId)
    {
        var companyId = Guid.Parse(User.FindFirstValue("companyId")!);

        var store = await _storeService.GetByIdAsync(storeId, companyId);

        if (store is null)
            return NotFound();

        return Ok(store);
    }

    [HttpGet]
    public Task<IActionResult> GetAll()
    {
        throw new NotImplementedException();
    }
}