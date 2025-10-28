using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class ProductInfo2
    {
        public long? packageID { get; set; }
        public string p { get; set; }
        public string barcode { get; set; }
        public string PkgNo { get; set; }
        public int? InvID { get; set; }
        public string i { get; set; }
        public long w { get; set; }
        public bool? IsNew { get; set; }
        public int? Prodid { get; set; }
        public decimal? pricetax { get; set; }
        public decimal? price { get; set; }
        public int pricetype { get; set; }
        public int? imagelink { get; set; }
        public string measuremane { get; set; }
        public decimal? qty { get; set; }
        public bool? IsPriceless { get; set; }
        public decimal? PWeight { get; set; }
        public decimal? Pwidth { get; set; }
        public decimal? PHeight { get; set; }
        public decimal? PLength { get; set; }
        public string PriceList { get; set; }
        public decimal ActivePrice { get; set; }
        public string PointCategory { get; set; }
        public bool? IsSale { get; set; }
        public string GuaranteePlan { get; set; }
        public string GroupCode { get; set; }
        public string UPC { get; set; }
        public string SKU { get; set; }
        public string imageurl { get; set; }
        public decimal discount { get; set; }
    }
}