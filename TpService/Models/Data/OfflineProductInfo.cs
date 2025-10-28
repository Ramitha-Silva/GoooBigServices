using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class OfflineProductInfo
    {
        public string  product_id { get; set; }
        public string  product_name { get; set; }
        public decimal? product_quantity { get; set; }
        public decimal cost_price { get; set; }
        public decimal retail_price { get; set; }
        public decimal total_cost_price { get; set; }
        public decimal total_retail_price { get; set; }
        public string category { get; set; }
        public string supplier { get; set; }
        public string imagename { get; set; }
        public int discount { get; set; }
        public decimal taxapply { get; set; }
        public string Shopid { get; set; }
        public int status { get; set; }
        public string description { get; set; }
        public string weight { get; set; }
        public string mdate { get; set; }
        public string edate { get; set; }
    }
}

 