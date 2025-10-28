using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels.RecievedInvoice
{
    public class RecievedInvoicePaymentType
    {
        public int? PaymentType { get; set; }
        public decimal? Amount { get; set; }
        public string Details { get; set; }

        //public decimal Cash { get; set; }
        //public decimal Visa { get; set; }
        //public decimal Master { get; set; }
        //public decimal Mada { get; set; }
        //public decimal Qitaf { get; set; }
        //public decimal STCPay { get; set; }
        //public decimal Points { get; set; }
        //public decimal Coupon { get; set; }
        //public decimal Remain { get; set; }


        //public string QitafInfo { get; set; }
        //public string STCPayInfo { get; set; }
        //public string PointsInfo { get; set; }
        //public string CouponInfo { get; set; }
    }
}