using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class SalesInvoice
    {
        public int Id { get; set; }
        public string InvNumber { get; set; }
        public DateTime InvDate { get; set; }
        public int SalesmanId  { get; set; }

        public List<InvProducts> Products { get; set; }
    }
}