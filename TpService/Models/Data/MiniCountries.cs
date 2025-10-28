using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class MiniCountries
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrash { get; set; }
    }

    public class ServerLinks
    {
        public string ApiLink1 { get; set; }
        public string ApiLink2 { get; set; }
        public string ApiLink3 { get; set; }
    }

    public class PackageNote
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}