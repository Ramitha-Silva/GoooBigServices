using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class GetInventoriesViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public string VatNumber { get; set; }
        public string ReportFooter { get; set; }
        public string ReportHeader { get; set; }
        public string ExtraInfo { get; set; }
        public string BackgroundLink { get; set; }
        public string IconLink { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
    }

    public class GetInstructionsViewModel
    {
        public int InventoryId { get; set; }
        public string MacAdrress { get; set; }
        public bool? IsUploadBill { get; set; }
        public bool? IsCallManager { get; set; }
        public bool? IsUpdateUsers { get; set; }
        public bool? IsUpdateProducts { get; set; }
        public bool? IsUpdatePrices { get; set; }
        public bool? IsUpdateOffers { get; set; }
        public bool? IsLogOut { get; set; }
        public bool? IsBackup { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsAutoUpload { get; set; }
        public bool? IsScreenBarcode { get; set; }
        public bool? IsScreenChoice { get; set; }

        public bool? IsArchive { get; set; }
    }
}