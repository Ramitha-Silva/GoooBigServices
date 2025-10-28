using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class OnlineProduct
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Qnt { get; set; }
        public decimal Total { get; set; }
        public string ImgLink { get; set; }
    }

    public enum OnlineProductStatus
    {
        Active = 1,
        Completed,
        Suspend,
        Canceled
    }
}