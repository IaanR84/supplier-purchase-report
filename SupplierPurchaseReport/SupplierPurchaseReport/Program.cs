using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;

var connectionString = "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;";

var purchaseRepository = new PurchaseRepository(connectionString);
var csvExportService = new CsvExportService();
var emailService = new EmailService(
    smtpServer: "YOUR_SMTP_SERVER",
    smtpPort: 587,
    username: "YOUR_EMAIL",
    password: "YOUR_PASSWORD"
);

var reportService = new SupplierReportService(
    purchaseRepository,
    csvExportService,
    emailService
);

await reportService.RunDailyReport(
    supplierName: "Supplier1",
    month: DateTime.Now.Month,
    year: DateTime.Now.Year,
    recipientEmail: "supplier@example.com"
);

Console.WriteLine("Report sent successfully.");