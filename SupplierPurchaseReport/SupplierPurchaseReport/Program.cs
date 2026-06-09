using Microsoft.Extensions.Configuration;
using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;
using SupplierPurchaseReport.Settings;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/supplier-report-.txt",
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();


var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var emailSettings = config
    .GetSection("Email")
    .Get<EmailSettings>()!;

var connectionString = config.GetConnectionString("DefaultConnection")!;

if (!ValidateSettings(connectionString, emailSettings))
    return 1;

var services = new ServiceCollection();

services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
services.AddSingleton<IPurchaseRepository>(
    new PurchaseRepository(connectionString));
services.AddSingleton<ISupplierRepository>(
    new SupplierRepository(connectionString));
services.AddSingleton<ICsvExportService, CsvExportService>();
services.AddSingleton<IEmailService>(new EmailService(
    smtpServer: emailSettings.SmtpServer,
    smtpPort: emailSettings.SmtpPort,
    username: emailSettings.Username,
    password: emailSettings.Password,
    from: emailSettings.From
));
services.AddSingleton<ISupplierReportService, SupplierReportService>();

var provider = services.BuildServiceProvider();
var reportService = provider.GetRequiredService<ISupplierReportService>();
var supplierRepository = provider.GetRequiredService<ISupplierRepository>();

var suppliers = await supplierRepository.GetAllSuppliers();

var successCount = 0;
var failCount = 0;

foreach (var supplier in suppliers)
{


    try
    {
        await reportService.RunDailyReport(
            supplierName: supplier.Name,
            month: DateTime.Now.Month,
            year: DateTime.Now.Year,
            recipientEmail: supplier.RecipientEmail
            
        );
        successCount++;
    } catch (Exception ex)
    {
        Log.Error(ex, $"Error occurred while processing supplier '{supplier.Name}'");
        failCount++;
    }
}

Log.Information($"Successful reports: {successCount}, Failed reports: {failCount}");

Log.CloseAndFlush();

return 0;
static bool ValidateSettings(string? connectionString, EmailSettings? emailSettings)
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
        var errorList = string.Join(", ", errors);
        Log.Error("Startup validation failed. Missing settings: {Settings}", errorList);
        Log.CloseAndFlush();
        return false;
    }

    return true;
}



