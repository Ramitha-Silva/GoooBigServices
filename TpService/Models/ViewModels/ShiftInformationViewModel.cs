using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class ShiftInformationViewModel
    {
        public decimal? Deposit { get; set; }
        public decimal? Withdraw { get; set; }
        public decimal? InvoiceReturn { get; set; }
        public decimal? StartAmount { get; set; }


        public decimal? Cash { get; set; }
        public decimal? Visa { get; set; }
        public decimal? Mada { get; set; }
        public decimal? MasterCard { get; set; }
        public decimal? Qitaf { get; set; }
        public decimal? Points { get; set; }
        public decimal? Stcpay { get; set; }
        public decimal? Coupon { get; set; }
        public decimal? Remain { get; set; }

        public decimal? TotalPox { get; set; }
        public decimal? TotalSales { get; set; }

        public decimal? Total { get; set; }


    }
}