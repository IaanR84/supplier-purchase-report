using Microsoft.Extensions.Configuration;
using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;
using SupplierPurchaseReport.Settings;


// 1. Build the configuration object
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var emailSettings = config
    .GetSection("Email")
    .Get<EmailSettings>();


// 2. Read values out by their path
var connectionString = config.GetConnectionString("DefaultConnection");

var purchaseRepository = new PurchaseRepository(connectionString);
var supplierRepository = new SupplierRepository(connectionString);
var csvExportService = new CsvExportService();
var emailService = new EmailService(
    smtpServer: emailSettings.SmtpServer,
    smtpPort: emailSettings.SmtpPort,
    username: emailSettings.Username,
    password: emailSettings.Password
);

var reportService = new SupplierReportService(
    purchaseRepository,
    csvExportService,
    emailService
);

var suppliers = await supplierRepository.GetAllSuppliers();

var successCount = 0;
var failCount = 0;

foreach (var supplier in suppliers)
{


    try
    {
        await reportService.RunDailyReport(
            supplierName: supplier.SupplierName,
            month: DateTime.Now.Month,
            year: DateTime.Now.Year,
            recipientEmail: supplier.RecipientEmail
            
        );
        successCount++;
    } catch (Exception ex)
    {
        Console.WriteLine($"Error occurred while processing supplier '{supplier.SupplierName}': {ex.Message}");
        failCount++;
    }
}

Console.WriteLine($"Successful reports: {successCount}, Failed reports: {failCount}");
