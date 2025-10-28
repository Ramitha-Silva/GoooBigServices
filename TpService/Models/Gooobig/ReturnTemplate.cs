using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Gooobig
{
    public class ReturnTemplate<T1,T2>
    {
        public IEnumerable<T1> List1 { get; set; }
        public IEnumerable<T2> List2 { get; set; }
    }
}