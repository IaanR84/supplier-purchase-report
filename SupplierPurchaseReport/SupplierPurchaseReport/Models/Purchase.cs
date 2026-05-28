using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierPurchaseReport.Models
{
    public class Purchase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DatePurchased { get; set; }
        public string Branchnr { get; set; }
        public string BranchName { get; set; }
        public decimal Amount { get; set; }


    }
}
