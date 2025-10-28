using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{

    public class ShiftViewModel
    {
        public long Id { get; set; }
        public int? UserID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsActive { get; set; }
        public decimal? StartAmount { get; set; }
        public decimal? EndAmount { get; set; }
        public int? ReceivedFrom { get; set; }
        public int? DepositTo { get; set; }
        public int? WebsiteID { get; set; }
        public bool? IsComplete { get; set; }
        public int? InventoryId { get; set; }
    }

    public class PoxViewModel
    {
        public int? Id { get; set; }
        public int? ShiftID { get; set; }
        public DateTime? ProcessDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? ProcessType { get; set; }
        public int? EmpFrom { get; set; }
        public int? EmpTo { get; set; }
        
    }

    public class barcodemissingModel
    {
        public int? autoid { get; set; }
        public int? Id { get; set; }
        public string Barcode { get; set; }
        public int? SalesmanID { get; set; }
        public int? ShiftID { get; set; }
    }

    public class RequestCSIDModel
    {
        public int? OTP { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string CSR { get; set; }
        public int? requestID { get; set; }
        public string dispositionMessage { get; set; }
        public string binarySecurityToken { get; set; }
        public string secret { get; set; }
        public string DeviceID { get; set; }
    }

    public class complianceCSIDModel
    {
        public string csr { get; set; }
    }

    public enum Mode
    {
        developer,
        Simulation,
        Production
    }
}