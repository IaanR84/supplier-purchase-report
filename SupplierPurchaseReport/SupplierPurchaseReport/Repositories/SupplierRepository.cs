using Microsoft.Data.SqlClient;
using SupplierPurchaseReport.Models;
using Dapper;


namespace SupplierPurchaseReport.Repositories
{
    public class SupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Supplier>> GetAllSuppliers()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
            Select SupplierName,
                   RecipientEmail
                from Supplier";

            var suppliers = await connection.QueryAsync<Supplier>(sql);
            
            return suppliers.ToList();



        }

    }

}