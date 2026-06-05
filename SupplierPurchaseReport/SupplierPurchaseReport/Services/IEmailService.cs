using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierPurchaseReport.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string fileName, byte[] attachment);
    }
}
