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


        public static ReportResult ValidationFailure(string message, int month, int year)
        {
            return new ReportResult
            {
                Month = month,
                Year = year,
                Message = message,
                Success = false
                // SupplierName, RecipientEmail stay string.Empty — fine, never looked at
                // GeneratedAt stays DateTime.MinValue — fine, irrelevant here
            };
        }
        public static ReportResult NotFound(string message, int month, int year)
        {
            return new ReportResult
            {
                Month = month,
                Year = year,
                Message = message,
                Success = false
            };
        }
    }
}
