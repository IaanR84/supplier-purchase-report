# Supplier Purchase Report

A C# service that automatically generates and emails daily purchase reports to suppliers — replacing a manual process across 10+ suppliers and 100+ daily member transactions.

---

## What it does

Each day, member purchases made at a specific supplier are extracted from the database, formatted into a CSV file, and emailed directly to that supplier. The entire process runs with a single method call.

Previously this was done manually. This system makes it automatic, repeatable, and supplier-agnostic — the same code runs for any supplier without modification.

---

## How it works

The system is built as four separate service classes, each with a single responsibility:

| Class | Responsibility |
|---|---|
| `Purchase` | Defines the shape of a purchase record |
| `PurchaseRepository` | Fetches purchases from SQL Server by supplier and date |
| `CsvExportService` | Formats and exports purchase data to CSV |
| `EmailService` | Attaches the CSV and sends it via SMTP |
| `SupplierReportService` | Orchestrates the full process end-to-end |

### Running a report

```csharp
await supplierReportService.RunMonthlyReport(
    "supplier1",
    5,
    2026,
    "info@supplier1.co.za");
```

One call fetches the data, builds the file, and delivers it.

---

## Tech stack

- **C#** — .NET backend
- **Dapper** — lightweight SQL mapping
- **SQL Server** — data source
- **SMTP** — email delivery

---

## Project structure

```
SupplierPurchaseReport/
├── Models/
│   └── Purchase.cs
├── Repositories/
│   └── PurchaseRepository.cs
├── Services/
│   ├── CsvExportService.cs
│   ├── EmailService.cs
│   └── SupplierReportService.cs
```

---

## Key design decisions

**Dependency injection throughout** — services receive their dependencies through constructors. No service creates its own dependencies. This makes each class independently testable and easy to swap out.

**Formatting separated from data access** — `PurchaseRepository` returns raw `DateTime` and `decimal` values. `CsvExportService` handles all formatting. A change in the supplier's required date format touches one class only.

**Parameterized queries only** — no values are hardcoded into SQL strings. All filtering is done through parameters, protecting against SQL injection.

---

## What comes next

- Purchase trend analysis — compare supplier volumes across months
- Multi-supplier batch runner — trigger all suppliers in one scheduled job
- REST API layer — expose report generation as an HTTP endpoint
- Blazor dashboard — view purchase summaries without running a query

---

## About this project

This is the first project in my transition from SQL development to C# backend development. It is built from a real business problem I solved at work — converted from a stored procedure into a proper service layer.

Every line is understood. Nothing was copy-pasted without knowing why.
