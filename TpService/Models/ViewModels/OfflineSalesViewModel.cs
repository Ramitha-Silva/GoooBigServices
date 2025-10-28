using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class OfflineSalesViewModel
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        public  decimal Qnt { get; set; }

        [Required]
        public  decimal Price { get; set; }

        [Required]
        public  decimal Tax { get; set; }

        [Required]
        public  decimal Discount { get; set; }

        [Required]
        public  decimal Total { get; set; }

        [Required]
        public  int InvoiceType { get; set; }

        [Required]
        public  long QueueID { get; set; }

        [Required]
        public  int UserId { get; set; }

        [Required]
        public  DateTime CreatedDate { get; set; }

        [Required]
        public  int InventoryID { get; set; }

        [Required]
        public  int WebsiteID { get; set; }
        public string ClientName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}