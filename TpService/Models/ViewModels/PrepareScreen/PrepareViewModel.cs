using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels.PrepareScreen
{
    public class PrepareViewModel
    {
        public int Id { get; set; }
        public string    Name { get; set; }
        public string    CreatedDate { get; set; }
        public string    CreatedTime { get; set; }
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Qnt { get; set; }
        public decimal Price { get; set; }
    }

    public class SliderContent
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<SliderImages> data { get; set; }
        public int fav_count { get; set; }
        public int cart_count { get; set; }
    }

    public class SliderImages
    {
        public int id { get; set; }
        public string image { get; set; }
    }

    public class ProductList
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string status { get; set; }
    }

    public class PackageListMini
    {
        public string AutoId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Price { get; set; }
    }

    public class PackageList
    {
        public int id { get; set; }
        public int category_id { get; set; }
        public int brand_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string price { get; set; }
        public int quantity { get; set; }
        public string cgst { get; set; }
        public string sgst { get; set; }
        public string igst { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int selected_index { get; set; }
        public int SecondLoad { get; set; }
        public int checkPickerLoad { get; set; }
        public string favourite { get; set; }
        public List<PackageVariations> get_product_variations { get; set; }
        public List<PackageBrand> product_brand { get; set; }
    }

    public class PackageVariations
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int unit_id { get; set; }
        public int weight { get; set; }
        public string price { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public List<PackageUnit> product_units { get; set; }
    }

    public class PackageBrand
    {
        public int id { get; set; }
        public string brand_name { get; set; }
    }

    public class PackageUnit
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Users
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile_number { get; set; }
        public string password { get; set; }
        public string profile_picture { get; set; }
        public string device_id { get; set; }
        public string device_token { get; set; }
        public string device_type { get; set; }
        public string full_address { get; set; }
        public string address_type { get; set; }
        public string referral_code { get; set; }
        public string is_verified { get; set; }
        public string user_status { get; set; }
        public string user_type { get; set; }
        public string status { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public long websiteid { get; set; }
    }

    public class UsersRegister
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

    public class UserLogin
    {
        public int id { get; set; }
        public string name { get; set; }
        public string mobile_number { get; set; }
    }

    public class UsersLogin
    {
        public string email { get; set; }
        public string mobile_number { get; set; }
        public string password { get; set; }
        public long websiteid { get; set; }
    }

    public class FavoriteList
    {
        public int id { get; set; }
        public string product_name { get; set; }
        public string image { get; set; }
        public string brand_name { get; set; }
        public string price { get; set; }
    }

    public class OrderList
    {
        public string order_code { get; set; }
        public string order_total { get; set; }
        public int cart_id { get; set; }
        public string from_date { get; set; }
        public string total_without_tax { get; set; }
        public string total_with_tax { get; set; }
        public string to_date { get; set; }
        public string product_name { get; set; }
        public int product_id { get; set; }
        public int product_variation_id { get; set; }
        public int quantity { get; set; }
        public string image { get; set; }
        public string brand_name { get; set; }
        public string full_address { get; set; }
        
    }

    public class AddressList
    {
        public int id { get; set; }
        public string name { get; set; }
        public string mobile_number { get; set; }
        public string address_type { get; set; }
        public int default_address { get; set; }
        public string house_no { get; set; }
        public string street_details { get; set; }
        public string apartment_name { get; set; }
        public string landmark_details { get; set; }
        public string area_details { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pincode { get; set; }
        public string full_address { get; set; }
        
    }

    public class UserAddressList
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Details { get; set; }
        public string LocationH { get; set; }
        public string LocationV { get; set; }
    }

    public class GeideaCallBack
    {
        public string paymentId { get; set; }
        public string orderReference { get; set; }
        public string orderId { get; set; }
        public string status { get; set; }
        public string signature { get; set; }
        public string tokenId { get; set; }
        public string maskedCardNumber { get; set; }
        public string cardExpiryDate { get; set; }
        public string cardType { get; set; }
        public string customerEmail { get; set; }
    }

    public class GeideaEinvoice
    {
        public string status { get; set; }
        public string message { get; set; }
        public string paymentId { get; set; }
    }

    public enum IViewType
    {
        Silder, HorizontalCategoriesList, HorizontalCardProductList, VerticalCardProductList, None
    }
    public enum ItemType
    {
        Silder, Category, Product, Brand, None
    }

    public class GeneralItem
    {
        public string Eng_Name { get; set; } // Mandatary Name Of Item In case of English App Interface
        public string AR_Name { get; set; }// Mandatary Name Of Item In case of Arabic App Interface
        public string Eng_Brand { get; set; }// Optional Brand Of Item In case of English App Interface "Only Return In Case of Product"
        public string AR_Brand { get; set; }// Optional Brand Of Item In case of English App Interface "Only Return In Case of Product"
        public string Image { get; set; } // Mandatary Image URL Should always display image regardless of kind of item
        public decimal Price { get; set; } // Optional Price Of Item In case of English App Interface "Only Return In Case of Product"
        public long Id { get; set; } // Mandatary ID of item
        public decimal Qnt { get; set; }
    }

    public class ItemInfo
    {
        public string Eng_Name { get; set; } 
        public string AR_Name { get; set; }
        public string Eng_Desc { get; set; }
        public string AR_Desc { get; set; }
        public string Eng_Brand { get; set; }
        public string AR_Brand { get; set; }
        public string PkgNo { get; set; }
        public string BarCode { get; set; }
        public string SKU { get; set; }
        public List<string> Image { get; set; } 
        public decimal Price { get; set; }
        public decimal PriceBefore { get; set; }
        public string Status { get; set; }
        public decimal Rating { get; set; }
        public List<ItemInfoComments> Comments { get; set; }
        public List<PackageGroupList> PackageGroup { get; set; }
        public long Id { get; set; }
        public decimal Weight { get; set; }

    }

    public class ItemInfoComments
    {
        public string UserName { get; set; }
        public string Details { get; set; }
        public DateTime? RatingDate { get; set; }
        public decimal? RatingGrade { get; set; }

    }

    public class WarehouseProducts
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
        public string ExtraInfo { get; set; }
        public List<GeneralItem> ItemInfo { get; set; }
    }

    public class WProductsSummary
    {
        public int PackageId { get; set; }
        public decimal Qnt { get; set; }

    }

    public class PackageGroupList
    {
        public int Id { get; set; }
        public string UnitName { get; set; }

    }

    public class HomeDynamicControl
    {
        public int Order { get; set; } // Mandatary : Order Section to display on Home page
        public long ControlId { get; set; } // Mandatary : Section ID This will be used in function GetItems to Retrieve Items List
        public IViewType ViewType { get; set; } // Mandatary : Enum Control Type Template{ Silder, HorizontalCategoriesList, HorizontalCardProductList, VerticalCardProductList}
        public ItemType ItemType { get; set; } // Mandatary : Enum Kind of Items To Dispaly { Silder, Category, Product, Brand}
        public string Ar_Title { get; set; } //  Section Arabic Title "Can Be Empty to ignore title"
        public string Eng_Title { get; set; } // Section English Title "Can Be Empty to ignore title"
        public double? ItemWidth { get; set; } // Optional Element Width "Right now ignore it"
        public double ItemHeight { get; set; }// Optional Element Height "Right now ignore it"

        //HorizontalCategoriesList This Section might show categories, Brands and other like icons with name
        // Others are clear

    }
    public class StoresInfo
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

    public class ShippingCompanies
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PackageShippingCompanies
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
    }

}