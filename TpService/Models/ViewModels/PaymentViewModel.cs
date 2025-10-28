using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class PaymentViewModel
    {
        public decimal cash { get; set; }
        public decimal mada { get; set; }
        public decimal visa { get; set; }
        public decimal mastercard { get; set; }
        public decimal remain { get; set; }
        public decimal qitaf { get; set; }
        public decimal coupon { get; set; }
        public decimal point { get; set; }
        public decimal stcpay { get; set; }

        public string qitafinfo { get; set; }
        public string couponinfo { get; set; }
        public string pointinfo { get; set; }
        public string stcpayinfo { get; set; }

    }
}