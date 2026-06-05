using SupplierPurchaseReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierPurchaseReport.Repositories
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllSuppliers();
    }
}
