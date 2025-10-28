using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.ViewModels
{
    public class BuffetOrderViewModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public List<SubBuffetOrderViewModel> OrderDetails { get; set; }
     

    }

    public class SubBuffetOrderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int OrderId { get; set; }

    }
}