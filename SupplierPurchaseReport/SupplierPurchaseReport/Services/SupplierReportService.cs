using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;

namespace SupplierPurchaseReport.Services
{
    public class SupplierReportService: ISupplierReportService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ICsvExportService _csvExportService;
        private readonly IEmailService _emailService;

        public SupplierReportService(
      IPurchaseRepository purchaseRepository,
      ICsvExportService csvExportService,
      IEmailService emailService)
        {
            _purchaseRepository = purchaseRepository;
            _csvExportService = csvExportService;
            _emailService = emailService;
        }

        public async Task RunDailyReport(
            string supplierName, int month, int year, string recipientEmail)
        {
            var purchases = await _purchaseRepository
                .GetBySupplier(supplierName, month, year);

            var csvFile = _csvExportService
                .Export(purchases);

            await _emailService
                .SendAsync(recipientEmail,
                           $"Daily Purchases - {supplierName}",
                           $"{supplierName}_{month}_{year}.csv",
                           csvFile);
        }
    }
}