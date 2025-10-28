using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class ObjTop10
    {
        public int autoID { get; set; }
        public string name { get; set; }
        public string imagelink { get; set; }
        public int total { get; set; }
    }

    public class ObjLastOps5
    {
        public string CreatedDate { get; set; }
        public string PageName { get; set; }
        public string OperationName { get; set; }
    }

    public class ObjVisitedPage
    {
        public string PgName { get; set; }
        public string PgLink { get; set; }
        public int CacheCount { get; set; }
    }

    public class ObjFavoritePage
    {
        public string ArabicName { get; set; }
        public string PageLink { get; set; }
        public string Details { get; set; }
    }

}