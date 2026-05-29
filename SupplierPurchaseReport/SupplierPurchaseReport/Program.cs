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

var reportSettings = config
    .GetSection("Report")
    .Get<ReportSettings>();



// 2. Read values out by their path
var connectionString = config.GetConnectionString("DefaultConnection");

var purchaseRepository = new PurchaseRepository(connectionString);
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

await reportService.RunDailyReport(
    supplierName: reportSettings.SupplierName,
    month: DateTime.Now.Month,
    year: DateTime.Now.Year,
    recipientEmail: reportSettings.RecipientEmail
);

Console.WriteLine("Report sent successfully.");