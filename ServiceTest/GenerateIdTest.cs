using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ServiceTest
{
    [TestClass]
    public class GenerateIdTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        public static string GenerateID(int TypeID, int MyWebsiteID)
        {
            using (var db = new TpConnection())
            {
                string AutoNum = string.Empty;
                var YearID2 = db.FnFinYears.Where(x => x.WebsiteID == MyWebsiteID && x.IsCurrentYear == true && x.IsTrash == false).Select(x => x.YearNum).SingleOrDefault();
                string _YearNum = (YearID2.HasValue) ? YearID2.ToString() : "";
                string Tag2 = MyWebsiteID.ToString() + "" + _YearNum.Substring(_YearNum.Length - 2);

                if (TypeID == 9)
                {
                    string Tag = "SI";
                    var query = db.ScSalesInvoices.Where(x => x.WebsiteID == MyWebsiteID && x.IsTrash == false && x.IsActive == true).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = _YearNum.Substring(_YearNum.Length - 2);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                return AutoNum;
            }
        }
    }
}
