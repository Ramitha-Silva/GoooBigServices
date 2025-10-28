using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels.RecievedInvoice
{
    public class RecievedInvoice
    {
        public string InvoiceCode { get; set; }
        public string PriceCode { get; set; }
        public int InvoiceType { get; set; }
        public int? SalesmanID { get; set; }
        public int? SupplierNumber { get; set; }
        public int InvoicesStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public int InventoryId { get; set; }
        public string InvoiceDetails { get; set; }
        public int? ShippingStatus { get; set; }
        public int? PaymentType { get; set; }
        public int? ShippingPackageId { get; set; }
        public decimal ShippingPrice { get; set; }
        public int UserId { get; set; }
        public int? WebsiteId { get; set; }
        public int? ShiftId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string ClientAccountNo { get; set; }
        public string ClientAddress { get; set; }
        public string ClientCode { get; set; }
        public List<RecievedInvoiceService> RecievedInvoiceServices { get; set; }
        public List<RecievedInvoiceProduct> RecievedInvoiceProducts { get; set; }
        public List<RecievedInvoicePaymentType> RecievedInvoicePaymentType { get; set; }
        public List<RecievedInvoiceTax> RecievedInvoiceTax { get; set; }
    }
    public class RecievedInvoiceTax
    {
        public int? InvoiceId { get; set; }
        public int? Id { get; set; }
        public int? TaxTypeID { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPrecentage { get; set; }
        public string Details { get; set; }
    }
    public class InventoryTrans
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string OfflineName { get; set; }
        public string PriceCode { get; set; }
        public int EmpIDFrom { get; set; }
        public int EmpIDTo { get; set; }
        public int InventoryIDFrom { get; set; }
        public int InventoryIDTo { get; set; }
        public string EmpNameFrom { get; set; }
        public string EmpNameTo { get; set; }
        public string InventoryNameFrom { get; set; }
        public string InventoryNameTo { get; set; }
        public DateTime? TransferDate { get; set; }
        public int StatusFrom { get; set; }
        public int StatusTo { get; set; }
        public int StatusTransfer { get; set; }
        public string Details { get; set; }
        public int UserId { get; set; }
        public int? WebsiteId { get; set; }
        public List<RecievedInventoryProduct> RecievedInvoiceProducts { get; set; }

        //For InventoryTrans class for function getInvetoriesTransfer i need from u to add those fields as text you sent for readonly and display: public string EmpNameFrom { get; set; } , public string EmpNameTo { get; set; } ,public string InventoryNameFrom { get; set; } ,public string InventoryNameTo { get; set; }
        //for RecievedInvoiceProducts items list just add one more property public string ProductName { get; set; }

    }

    public class RecievedInventoryProduct
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Qnt { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductDiscount { get; set; }
        public decimal ProductTax { get; set; }
        public decimal TotalPrice { get; set; }
        public string Details { get; set; }
    }
}