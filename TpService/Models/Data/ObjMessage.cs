using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class ObjMessage
    {
        public long autoID { get; set; }
        public string ImgBG { get; set; }
        public string FullName { get; set; }
        public string Message { get; set; }
        public DateTime UpdatedDate { get; set; }
    }


}