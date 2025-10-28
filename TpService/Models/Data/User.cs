using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class User
    {
        public string Name { get; set; }
        public string Father_name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string DOB { get; set; }
        public string Username { get; set; }
        public string password { get; set; }
        public string usertype { get; set; }
        public string position { get; set; }
        public string imagename { get; set; }
        public string Shopid { get; set; }
        public int srstyle { get; set; }
        public int userId { get; set; }
    }
}