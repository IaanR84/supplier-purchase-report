using Microsoft.AspNetCore.Mvc;
using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;

namespace SupplierPurchaseReport.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierReportService _supplierReportService;

    public SuppliersController(
        ISupplierRepository supplierRepository,
        ISupplierReportService supplierReportService)
    {
        _supplierRepository = supplierRepository;
        _supplierReportService = supplierReportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierRepository.GetAllSuppliers();
        return Ok(suppliers);
    }

    [HttpPost("{id}/report")]
    public async Task<IActionResult> RunReport(
        string id,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var supplier = await _supplierRepository.GetSupplierById(id);

        if (supplier == null)
            return NotFound($"Supplier with ID {id} not found.");

        await _supplierReportService.RunDailyReport(
            supplierName: supplier.Name,
            month: month,
            year: year,
            recipientEmail: supplier.RecipientEmail
        );

        return Ok("Report triggered successfully.");
    }
}