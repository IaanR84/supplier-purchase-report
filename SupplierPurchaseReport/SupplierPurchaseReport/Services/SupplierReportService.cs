using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;
using Microsoft.Extensions.Logging;
using SupplierPurchaseReport.Models;

namespace SupplierPurchaseReport.Services
{
    
    public class SupplierReportService: ISupplierReportService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ICsvExportService _csvExportService;
        private readonly IEmailService _emailService;
        private readonly ILogger<SupplierReportService> _logger;

        public SupplierReportService(
      IPurchaseRepository purchaseRepository,
      ICsvExportService csvExportService,
      IEmailService emailService,
      ILogger<SupplierReportService> logger)
        {
            _purchaseRepository = purchaseRepository;
            _csvExportService = csvExportService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ReportResult> RunDailyReport(string supplierName, int month, int year, string recipientEmail)
        {
            var result = new ReportResult
            {
                SupplierName = supplierName,
                RecipientEmail = recipientEmail,
                Month = month,
                Year = year,
                GeneratedAt = DateTime.UtcNow
            };

            try
            {
                var purchases = await _purchaseRepository
                    .GetBySupplier(supplierName, month, year);

                if (purchases == null || !purchases.Any())
                {
                    result.Success = false;
                    result.Message = "No purchases found for this period.";
                    return result;
                }

                var csvFile = _csvExportService
                    .Export(purchases);

                await _emailService
                    .SendAsync(recipientEmail,
                               $"Daily Purchases - {supplierName}",
                               $"{supplierName}_{month}_{year}.csv",
                               csvFile);

                result.Success = true;
                result.Message = "Report generated and sent successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run daily report for {SupplierName}", supplierName);
                result.Success = false;
                result.Message = "An error occurred while generating the report.";
            }

            return result;
        }
    }
    
}