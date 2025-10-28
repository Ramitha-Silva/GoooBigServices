using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels.RecievedInvoice
{
    public class RecievedInvoiceProduct
    {
        public int Id { get; set; }
        public long ProductId { get; set; }
        public decimal Qnt { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductDiscount { get; set; }
        public decimal ProductTax { get; set; }
        public decimal TotalPrice { get; set; }
        public string Details { get; set; }
        public string SN { get; set; }
    }
}