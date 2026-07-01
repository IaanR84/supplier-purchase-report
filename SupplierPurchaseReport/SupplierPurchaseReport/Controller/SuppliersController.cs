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

    private static bool IsValidMonthYear(int month, int year)
    {
        return month >= 1 && month <= 12 && year >= 2000 && year <= 2100;
    }

    private async Task<Supplier?> FindSupplierOrNull(string supplierCode)
    {
        return await _supplierRepository.GetSupplierById(supplierCode);
    }

    [HttpPost("{id}/report")]
    public async Task<IActionResult> RunReport(
        string id,
        [FromQuery] int month,
        [FromQuery] int year)
    {

        if (!IsValidMonthYear(month, year))
        {
            return BadRequest(ReportResult.ValidationFailure("Invalid input. Month must be between 1 and 12, year between 2000 and 2100.", month, year));
        }

        var supplier = await FindSupplierOrNull(id);

        if (supplier == null)
            return NotFound(ReportResult.NotFound($"Supplier with ID {id} not found.", month, year));



        var reportResult = await _supplierReportService.RunDailyReport(
     supplierName: supplier.Name,
     month: month,
     year: year,
     recipientEmail: supplier.RecipientEmail
 );

        if (!reportResult.Success)
            return BadRequest(reportResult);

        return Ok(reportResult);
    }
}