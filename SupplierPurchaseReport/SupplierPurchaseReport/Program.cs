using Microsoft.Extensions.Configuration;
using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;
using SupplierPurchaseReport.Settings;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/supplier-report-.txt",
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var config = builder.Configuration;

var emailSettings = config
    .GetSection("Email")
    .Get<EmailSettings>()!;

var connectionString = config.GetConnectionString("DefaultConnection")!;

if (!ValidateSettings(connectionString, emailSettings))
{
    Log.CloseAndFlush();
    return;
}

builder.Services.AddControllers();
builder.Services.AddSingleton<IPurchaseRepository>(
    new PurchaseRepository(connectionString));
builder.Services.AddSingleton<ISupplierRepository>(
    new SupplierRepository(connectionString));
builder.Services.AddSingleton<ICsvExportService, CsvExportService>();
builder.Services.AddSingleton<IEmailService>(new EmailService(
    smtpServer: emailSettings.SmtpServer,
    smtpPort: emailSettings.SmtpPort,
    username: emailSettings.Username,
    password: emailSettings.Password,
    from: emailSettings.From
));
builder.Services.AddSingleton<ISupplierReportService, SupplierReportService>();

var app = builder.Build();

app.MapControllers();

app.Run();

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
        Log.Error("Startup validation failed. Missing settings: {Settings}",
            string.Join(", ", errors));
        return false;
    }

    return true;
}