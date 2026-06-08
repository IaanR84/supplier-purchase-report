using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierPurchaseReport.Models
{
    public class Purchase
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public required DateTime DatePurchased { get; set; }
        public required string Branchnr { get; set; }
        public required string BranchName { get; set; }
        public required decimal Amount { get; set; }


    }
}
