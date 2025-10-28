using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class VwUsersPagesPermissions
    {
        public int userid { get; set; }
        public int pageid { get; set; }
        public string pagecode { get; set; }
        public bool CanView { get; set; }
        public bool canaddnew { get; set; }
        public bool canedit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public bool CanSearch { get; set; }
        public bool CanBrowse { get; set; }
        public bool CanRemove { get; set; }
        public bool CanRestore { get; set; }
        public bool CanObserve { get; set; }
    }
    
}