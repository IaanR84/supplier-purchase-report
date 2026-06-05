using System.Text;
using SupplierPurchaseReport.Models;

namespace SupplierPurchaseReport.Services
{
    public class CsvExportService: ICsvExportService
    {
        public byte[] Export(List<Purchase> purchases)
        {
            var sb = new StringBuilder();

            sb.AppendLine("kode,Naam,Datum Transaksie,Boenr,BoeNaam,Bedrag");

            foreach (var purchase in purchases)
            {
                var line = string.Format("{0},{1},{2},{3},{4},{5}",
                    purchase.Code,
                    purchase.Name,
                    purchase.DatePurchased.ToString("yyyy/MM/dd"),
                    purchase.Branchnr,
                    purchase.BranchName,
                    purchase.Amount.ToString("F2"));

                sb.AppendLine(line);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}