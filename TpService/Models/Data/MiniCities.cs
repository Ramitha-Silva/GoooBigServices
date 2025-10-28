using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class StoresInformation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string LogoImage { get; set; }
        public string BackgroundImage { get; set; }
        public string VatNumber { get; set; }
        public string InvoiceHeader { get; set; }
        public string InvoiceFooter { get; set; }
        public string StandardColor { get; set; }
    }

    public class RegisterNewUser
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile_number { get; set; }
        public string password { get; set; }
        public string device_id { get; set; }
        public string device_token { get; set; }
        public string device_type { get; set; }
        public long websiteid { get; set; }
    }

    public class AllItem
    {
        public string eng_Name { get; set; } // Mandatary Name Of Item In case of English App Interface
        public string aR_Name { get; set; }// Mandatary Name Of Item In case of Arabic App Interface
        public string eng_Brand { get; set; }// Optional Brand Of Item In case of English App Interface "Only Return In Case of Product"
        public string aR_Brand { get; set; }// Optional Brand Of Item In case of English App Interface "Only Return In Case of Product"
        public string image { get; set; } // Mandatary Image URL Should always display image regardless of kind of item
        public decimal price { get; set; } // Optional Price Of Item In case of English App Interface "Only Return In Case of Product"
        public long id { get; set; } // Mandatary ID of item
        public decimal qnt { get; set; }
    }

    public class AllUserAddress
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public int countryId { get; set; }
        public string country { get; set; }
        public int cityId { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string details { get; set; }
        public string locationH { get; set; }
        public string locationV { get; set; }
    }

    public class WebsiteElement
    {
        public int AutoID { get; set; }
        public int? WebSiteID { get; set; }
        public int? SectionID { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public string Content { get; set; }
        public string ExternalLink { get; set; }
        public int? ServiceID { get; set; }
        public string Tags { get; set; }
        public bool? Level { get; set; }
        public bool? Templete { get; set; }
        public long? Picture1 { get; set; }
        public long? Picture2 { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? StartDate_ { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ViewCount { get; set; }
        public bool? CommentsAllow { get; set; }
        public string Icon { get; set; }
        public bool? IsTrash { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? OrderBy { get; set; }
        public string Code { get; set; }
    }

    public class MiniCities
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public int CountryId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
    }

    public class PaymentTypeCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Details { get; set; }

    }

    public class SpecialOffer
    {
        public int AutoID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OfferNumber { get; set; }
        public int? OfferType { get; set; }
        public decimal? Descount { get; set; }
        public int? DescountType { get; set; }
        public string Details { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }

    }

    public class SpecialOffer2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OfferNumber { get; set; }
        public int? OfferType { get; set; }
        public decimal? Descount { get; set; }
        public int? DescountType { get; set; }
        public string Details { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }

    }

    public class ClientInfo
    {
        public int AutoID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AccountNo { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
    }

    //    {
    //  "event": "app.store.token",
    //  "merchant": 1234918345,
    //  "created_at": "2021-10-05 16:41:07",
    //  "data": {
    //    "access_token": "kG7eCGY0QlrgNZK1zFQmRIifReqsKJ9GJquPvsnJhho.l5Msr8jD5GBxxxx",
    //    "expires": 1634661667,
    //    "refresh_token": "WYQz6bMeaonMZ6WjhrkMTRb7fSkrAVpLH5n1V0_X9eU.e5Gqz1ks8Q8dHxxxx",
    //    "scope": "settings.read offline_access",
    //    "token_type": "bearer"
    //  }
    //}
    public class SallaToken
    {
        public string @event { get; set; }
        public long merchant { get; set; }
        public string created_at { get; set; }
        public SallaTokenData data { get; set; }
    }

    public class SallaTokenData
    {
        public string access_token { get; set; }
        public long expires { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }

    public class SallaAuthData
    {
        public string code { get; set; }
        public string scope { get; set; }
        public string state { get; set; }
    }

    public class SallaAuthResponse
    {
        public string access_token { get; set; }
        public string expires { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }

    }
    public class SallaResponse
    {
        public string @event { get; set; }
        public int merchant { get; set; }
        public string created_at { get; set; }
        public SallaResponseData data { get; set; }
    }
    public class SallaResponseData
    {
        public string access_token { get; set; }
        public int expires { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }
    public class DeviceInfoData
    {
        public int opID { get; set; }
        public int InventoryID { get; set; }
        public string DPosVersion { get; set; }
        public string DMacAddress { get; set; }
        public string DWindowOperatingSystem { get; set; }
        public string DWindowNetFrameworkVersion { get; set; }
        public string DWindowNetFrameworkVersion2 { get; set; }
        public string DWindowProcessor { get; set; }
        public string DWindowRam { get; set; }
        public string DWindowLanIp { get; set; }
        public string DWindowsModel { get; set; }
        public string DSystemProcessors { get; set; }
        public string SubscriptionCode { get; set; }

    }

    public class ProductListStoreInformation
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
        public int? imagelink { get; set; }
        public string measuremane { get; set; }
        public decimal? qty { get; set; }
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
        public decimal? pricetax { get; set; }
        public bool? IsTaxGroup { get; set; }
        public int? TaxGroupId { get; set; }
        public int? pricetype { get; set; }
        public decimal? activeprice { get; set; }
        public string pricelist { get; set; }
        public decimal? price { get; set; }
        public decimal? taxamnt { get; set; }
        public decimal? pricewtax { get; set; }
        public int? SectionType { get; set; }
    }

    public class PointInfo
    {
        public int AutoID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int SalesID { get; set; }
        public int ShiftID { get; set; }
        public decimal Amount { get; set; }
        public decimal Points { get; set; }
        public string InvoiceName { get; set; }
        public long WebsiteID { get; set; }
    }

    public class SalesClientProfile
    {
        public int? ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int? City_id { get; set; }
        public string ClientBarcode { get; set; }
        public string ClientPricecode { get; set; }

    }

    public class ProducPrices
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long? PackageID { get; set; }
        public decimal? Price { get; set; }
        public string PriceCode { get; set; }
        public decimal? PriceLimit { get; set; }
        public int? PriceType { get; set; }
        public bool? PricewithTax { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }

    }

    public class ProducPricesInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long? PackageID { get; set; }
        public decimal? Price { get; set; }
        public string PriceCode { get; set; }
        public decimal? PriceLimit { get; set; }
        public int? PriceType { get; set; }
        public bool? PricewithTax { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long? WebsiteID { get; set; }

    }

    public class ProducInfo
    {
        public long? AutoID { get; set; }
        public string PkgName { get; set; }
        public string BarCode { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Price { get; set; }
        public int? PriceType { get; set; }
        public bool? PricewithTax { get; set; }

    }

    public class ProducInfoMeasure
    {
        public long? AutoID { get; set; }
        public string PkgName { get; set; }
        public string BarCode { get; set; }
        public decimal? MeasureCount { get; set; }
        public int? MeasureType { get; set; }
        public bool? IsMeasureMain { get; set; }

    }

    public class PricesCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsPrec { get; set; }
        public decimal? Precentage { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
    }
    public class SectionData
    {
        public int Id { get; set; }
        public List<OfSectionRelatedSection> SectionAdditional { get; set; }
        public List<OfSectionRelatedSection> SectionRelated { get; set; }

    }
    public class OfSectionRelatedSection
    {
        public int autoid { get; set; }
        public int SectionId { get; set; }
        public int RelatedSectionId { get; set; }
    }
    public class PricesCodeData
    {
        public int Id { get; set; }
        public List<PricesCodeInventoryInformation> PriceCodeInventory { get; set; }
        public List<PricesCodeProductInformation> PriceCodeProduct { get; set; }
        public List<PricesCodeClientInformation> PriceCodeClient { get; set; }
    }
    public class PricesCodeInventoryInformation
    {
        public int autoid { get; set; }
        public int CodeId { get; set; }
        public int InventoryId { get; set; }
    }
    public class PricesCodeProductInformation
    {
        public int autoid { get; set; }
        public int CodeId { get; set; }
        public int ProductId { get; set; }
    }
    public class PricesCodeClientInformation
    {
        public int autoid { get; set; }
        public int CodeId { get; set; }
        public int ClientId { get; set; }
    }

    public class PricesCodeProduct
    {
        public int Id { get; set; }
        public int? PriceCodeID { get; set; }
        public int? ProductID { get; set; }
    }

    public class SpecialOfferPrices
    {
        public int id { get; set; }
        public int? SpecialOfferID { get; set; }
        public long? PackageID { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Details { get; set; }

    }

    public class SpecialOfferSub
    {
        public int id { get; set; }
        public int? SpecialOfferID { get; set; }
        public long? PackageID { get; set; }
        public int? OfferProductsNo { get; set; }
        public decimal? Percentage { get; set; }
        public int? PercentageType { get; set; }
        public int? RequiredProductsNo { get; set; }
        public string Details { get; set; }
        public long? PackageOfferID { get; set; }
        public decimal? OfferLimit { get; set; }

    }

    public class TransactionTypeCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Details { get; set; }

    }

    public class MeasureTypeCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Details { get; set; }

    }

    public class InvoicesSynced
    {
        public string OfflineName { get; set; }
        public string OnlineName { get; set; }
    }
}