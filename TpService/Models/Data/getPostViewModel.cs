using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class getPostViewModel
    {
        public int AutoID { get; set; }
        public string Post { get; set; }
        public string CreatedDate { get; set; }
        public string NickName { get; set; }
        public string CreatedName { get; set; }
    }
}