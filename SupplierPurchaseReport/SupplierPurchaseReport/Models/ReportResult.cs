namespace SupplierPurchaseReport.Models
{
    public class ReportResult
    {
        public string SupplierName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
