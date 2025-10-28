using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class UsersResponse
    {
         
        public User Admin { get; set; }
        public User Manager { get; set; }
        public List<User> Users { get; set; }

    }
}