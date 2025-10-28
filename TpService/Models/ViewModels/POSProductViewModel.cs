using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    [Table("VwInvPackagePOSWithAutoID")]
    public class POSProductViewModel
    {
        [Key]
        public Guid AutoId { get; set; }
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
        public int? pricetype { get; set; }
        public int? imagelink { get; set; }
        public string measuremane { get; set; }
        public decimal? qty { get; set; }
        public bool? IsPriceless { get; set; }
        public decimal? PWeight { get; set; }
        public decimal? Pwidth { get; set; }
        public decimal? PHeight { get; set; }
        public decimal? PLength { get; set; }
        public string PriceList { get; set; }
        public decimal? ActivePrice { get; set; }
        public string sku { get; set; }
        public string upc { get; set; }
        public string groupcode { get; set; }
        public string guaranteeplan { get; set; }
        public bool? issale { get; set; }
        public string pointcategory { get; set; }
        public string imageurl { get; set; }
        public decimal? discount { get; set; }
        public DateTime updatedate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
        public string PColor { get; set; }


    }

    public class POSProductListViewModel
    {
        [Key]
        public Guid AutoId { get; set; }
        public long? PackageID { get; set; }
        public bool? IsPriceless { get; set; }
        public decimal? PWeight { get; set; }
        public decimal? Pwidth { get; set; }
        public decimal? PHeight { get; set; }
        public decimal? PLength { get; set; }
        public string p { get; set; }
        public string BarCode { get; set; }
        public string PkgNo { get; set; }
        public int? invid { get; set; }
        public string i { get; set; }
        public long w { get; set; }
        public bool? isnew { get; set; }
        public int? prodid { get; set; }
        public decimal? pricetax { get; set; }
        public decimal? price { get; set; }
        public int? pricetype { get; set; }
        public int? imagelink { get; set; }
        public string measuremane { get; set; }
        public decimal? qty { get; set; }
        public decimal? activeprice { get; set; }
        public string pricelist { get; set; }
        public string sku { get; set; }
        public string upc { get; set; }
        public string groupcode { get; set; }
        public string guaranteeplan { get; set; }
        public bool? issale { get; set; }
        public string pointcategory { get; set; }
        public string imageurl { get; set; }
        public decimal? discount { get; set; }
        public DateTime updatedate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
    }

    public class POSProductViewModelResult
    {
        public long? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string ProductNumber { get; set; }
        //public int? nventoryId { get; set; }
        public string InventoryName { get; set; }
        public long WebsiteId { get; set; }
        public bool? IsNew { get; set; }
        public int? SectionId{ get; set; }
        public decimal? PriceTax { get; set; }
        public decimal? Price { get; set; }
        public string PriceType { get; set; }
        public string Imagelink { get; set; }
        public string MeasureName { get; set; }
        public decimal? Qty { get; set; }
        public bool? IsPriceless { get; set; }
        public decimal? ProductWeight { get; set; }
        public decimal? Productwidth { get; set; }
        public decimal? ProductHeight { get; set; }
        public decimal? ProductLength { get; set; }
    }

    public class POSProductListViewModelResult
    {
        public long? PackageID { get; set; }
        public bool? IsPriceless { get; set; }
        public decimal? PWeight { get; set; }
        public decimal? Pwidth { get; set; }
        public decimal? PHeight { get; set; }
        public decimal? PLength { get; set; }
        public string p { get; set; }
        public string BarCode { get; set; }
        public string PkgNo { get; set; }
        public int? invid { get; set; }
        public string i { get; set; }
        public long w { get; set; }
        public bool? isnew { get; set; }
        public int? prodid { get; set; }
        public decimal? pricetax { get; set; }
        public decimal? price { get; set; }
        public int? pricetype { get; set; }
        public int? imagelink { get; set; }
        public string measuremane { get; set; }
        public decimal? qty { get; set; }
        public decimal? activeprice { get; set; }
        public string pricelist { get; set; }
        public string sku { get; set; }
        public string upc { get; set; }
        public string groupcode { get; set; }
        public string guaranteeplan { get; set; }
        public bool? issale { get; set; }
        public string pointcategory { get; set; }
        public string imageurl { get; set; }
        public decimal? discount { get; set; }
        public DateTime updatedate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
    }

    public class ProductListInformation
    {
        public long? PackageID { get; set; }
        public bool? IsPriceless { get; set; }
        public decimal? PWeight { get; set; }
        public decimal? Pwidth { get; set; }
        public decimal? PHeight { get; set; }
        public decimal? PLength { get; set; }
        public string p { get; set; }
        public string BarCode { get; set; }
        public string PkgNo { get; set; }
        public int? invid { get; set; }
        public string i { get; set; }
        public long w { get; set; }
        public bool? isnew { get; set; }
        public int? prodid { get; set; }
        public decimal? pricetax { get; set; }
        public decimal? price { get; set; }
        public int? pricetype { get; set; }
        public int? imagelink { get; set; }
        public string measuremane { get; set; }
        public decimal? qty { get; set; }
        public decimal? activeprice { get; set; }
        public string pricelist { get; set; }
        public string sku { get; set; }
        public string upc { get; set; }
        public string groupcode { get; set; }
        public string guaranteeplan { get; set; }
        public bool? issale { get; set; }
        public string pointcategory { get; set; }
        public string imageurl { get; set; }
        public decimal? discount { get; set; }
        public DateTime updatedate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
        public string PColor { get; set; }

    }
}