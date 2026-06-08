using System;
using System.Linq;


namespace SupplierPurchaseReport.Settings
{
    public class EmailSettings
    {

        public required string SmtpServer { get; set; }
        public required int SmtpPort { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

}

