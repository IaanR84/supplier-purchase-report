using SupplierPurchaseReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierPurchaseReport.Repositories
{
    public interface IPurchaseRepository
    {
        Task<List<Purchase>> GetBySupplier(string supplierName, int month, int year);
    }
}
