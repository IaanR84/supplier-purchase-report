using Microsoft.AspNetCore.Mvc;
using SupplierPurchaseReport.Repositories;

namespace SupplierPurchaseReport.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierRepository _supplierRepository;

    public SuppliersController(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierRepository.GetAllSuppliers();
        return Ok(suppliers);
    }
}