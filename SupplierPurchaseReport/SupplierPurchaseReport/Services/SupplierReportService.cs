using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;

namespace SupplierPurchaseReport.Services
{
    public class SupplierReportService
    {
        private readonly PurchaseRepository _purchaseRepository;
        private readonly CsvExportService _csvExportService;
        private readonly EmailService _emailService;

        public SupplierReportService(
            PurchaseRepository purchaseRepository,
            CsvExportService csvExportService,
            EmailService emailService)
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