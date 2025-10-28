using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class VwInventoryPackage
    {
        public long? PackageID { get; set; }
        public bool? IsPriceless { get; set; }
        public decimal pweight { get; set; }
        public decimal plength { get; set; }
        public decimal pheight { get; set; }
        public decimal pwidth { get; set; }
        public string p { get; set; }
        public string BarCode { get; set; }
        public string PkgNo { get; set; }
        public int invid { get; set; }
        public string i { get; set; }
        public int w { get; set; }
        public bool? isnew { get; set; }
        public int prodid { get; set; }
        public decimal pricetax { get; set; }
        public decimal price { get; set; }
        public int pricetype { get; set; }
        public string imagelink { get; set; }
        public string measuremane { get; set; }
        public decimal qty { get; set; }
        public decimal? activeprice { get; set; }
        public string pricelist { get; set; }
        public string sku { get; set; }
        public string upc { get; set; }
        public string groupcode { get; set; }
        public int? guaranteeplan { get; set; }
        public bool? issale { get; set; }
        public string pointcategory { get; set; }
        public string imageurl { get; set; }
        public decimal? discount { get; set; }
        
    }

    public class VwInventoryPackageList
    {
        public List<VwInventoryPackage> Product { get; set; }
    }
}