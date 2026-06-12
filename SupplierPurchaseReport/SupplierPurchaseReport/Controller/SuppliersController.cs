using Microsoft.AspNetCore.Mvc;
using SupplierPurchaseReport.Models;
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


        var result = new ReportResult
        {
            Month = month,
            Year = year,
            Success = false,
            Message = $"Supplier with ID {id} not found.",
            GeneratedAt = DateTime.UtcNow
        };

        if (supplier == null)
            return NotFound(result);  

        

        await _supplierReportService.RunDailyReport(
            supplierName: supplier.Name,
            month: month,
            year: year,
            recipientEmail: supplier.RecipientEmail
        );

        result = new ReportResult
        {
            SupplierName = supplier.Name,
            RecipientEmail = supplier.RecipientEmail,
            Month = month,
            Year = year,
            Success = true,
            Message = "Report triggered successfully.",
            GeneratedAt = DateTime.UtcNow
        };

        return Ok(result);
    }
}