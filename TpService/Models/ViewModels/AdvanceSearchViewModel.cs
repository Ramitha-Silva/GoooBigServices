using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class AdvanceSearchViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public string ClientName { get; set; }
        public decimal? Total { get; set; }
    }
}