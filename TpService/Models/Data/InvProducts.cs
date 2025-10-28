using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class InvProducts
    {
        public long ProductId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Qnt  { get; set; }
        public decimal  Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public int InventoryId { get; set; }
}
}