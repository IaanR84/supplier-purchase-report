using SupplierPurchaseReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SupplierPurchaseReport.Services
{
    public interface ISupplierReportService
    {
        Task<ReportResult> RunDailyReport(string supplierName, int month, int year, string recipientEmail);
    }
}
