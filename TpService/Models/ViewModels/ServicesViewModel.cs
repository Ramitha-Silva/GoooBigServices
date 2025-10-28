using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class ServicesViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal pricetax { get; set; }
        public int servicetype { get; set; }
    }
    public class OpOperationOrdersView
    {
        public int AutoID { get; set; }
        public string Name { get; set; }
        public string InvoiceNo { get; set; }
        public string ClientName { get; set; }
        public int? StatusID { get; set; }
        public string InvoiceDate { get; set; }
        public string CreatedDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CreatedTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}