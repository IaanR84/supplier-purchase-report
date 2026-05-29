using Microsoft.Extensions.Configuration;
using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;

// 1. Build the configuration object
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

// 2. Read values out by their path
var connectionString = config.GetConnectionString("DefaultConnection");

var purchaseRepository = new PurchaseRepository(connectionString);
var csvExportService = new CsvExportService();
var emailService = new EmailService(
    smtpServer: config["Email:SmtpServer"],
    smtpPort: int.Parse(config["Email:SmtpPort"]),
    username: config["Email:Username"],
    password: config["Email:Password"]
);

var reportService = new SupplierReportService(
    purchaseRepository,
    csvExportService,
    emailService
);

await reportService.RunDailyReport(
    supplierName: config["Report:SupplierName"],
    month: DateTime.Now.Month,
    year: DateTime.Now.Year,
    recipientEmail: config["Report:RecipientEmail"]
);

Console.WriteLine("Report sent successfully.");