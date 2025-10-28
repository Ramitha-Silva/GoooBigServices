using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class MiniInvoiceResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int InventoryId { get; set; }

    }
}