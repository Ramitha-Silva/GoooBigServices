using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels.RecievedInvoice
{
    public class RecievedInvoiceService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal PriceTax { get; set; }
        public decimal Total { get; set; }
        public int ServiceType { get; set; }
    }
}