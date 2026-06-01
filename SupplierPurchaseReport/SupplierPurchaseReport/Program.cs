using Microsoft.Extensions.Configuration;
using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;
using SupplierPurchaseReport.Settings;


var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var emailSettings = config
    .GetSection("Email")
    .Get<EmailSettings>();

var connectionString = config.GetConnectionString("DefaultConnection");

ValidateSettings(connectionString, emailSettings);

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

static void ValidateSettings(string? connectionString, EmailSettings? emailSettings)
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(connectionString))
        errors.Add("ConnectionStrings:DefaultConnection");

    if (emailSettings == null)
    {
        errors.Add("Email section (entire section missing)");
    }
    else
    {
        if (string.IsNullOrWhiteSpace(emailSettings.SmtpServer))
            errors.Add("Email:SmtpServer");

        if (emailSettings.SmtpPort == 0)
            errors.Add("Email:SmtpPort");

        if (string.IsNullOrWhiteSpace(emailSettings.Username))
            errors.Add("Email:Username");

        if (string.IsNullOrWhiteSpace(emailSettings.Password))
            errors.Add("Email:Password");
    }

    if (errors.Count > 0)
    {
        Console.WriteLine("Startup validation failed. The following settings are missing or invalid:");
        foreach (var error in errors)
            Console.WriteLine($"  - {error}");

        Environment.Exit(1);
    }
}