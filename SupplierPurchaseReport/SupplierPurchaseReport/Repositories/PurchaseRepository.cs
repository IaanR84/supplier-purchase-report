using Dapper;
using Microsoft.Data.SqlClient;
using SupplierPurchaseReport.Models;

namespace SupplierPurchaseReport.Repositories
{
    public class PurchaseRepository
    {
        private readonly string _connectionString;

        public PurchaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Purchase>> GetBySupplier(
            string supplierName, int month, int year)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT
                    k.Code,
                    k.Name,
                    t.DatePurchased,
                    t.BrancNr,
                    b.BrancName AS BranchNaam,
                    t.Amount
                FROM Purchases t
                JOIN Supplier k
                    ON k.Code = t.Supplier AND k.Tipe = 'K'
                JOIN Branch b
                    ON t.Code = b.Code
                   AND t.Branchnr = b.Branchnr
                WHERE k.Name LIKE @SupplierName
                AND MONTH(t.DatePurchased) = @Month
                AND YEAR(t.DatePurchased) = @Year";

            var purchases = await connection.QueryAsync<Purchase>(sql, new
            {
                SupplierName = $"%{supplierName}%",
                Month = month,
                Year = year
            });

            return purchases.ToList();
        }
    }
}