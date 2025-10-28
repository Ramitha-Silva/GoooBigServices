using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public class TPMngIDs
    {
        
        public static string GenerateID(int TypeID, int MyWebsiteID)
        {
            using (var db = new ShoppingCartConnection())
            {
                string AutoNum = string.Empty;
                var YearID2 = db.FnFinYears.Where(x => x.WebsiteID == MyWebsiteID && x.IsCurrentYear == true && x.IsTrash == false).Select(x => x.YearNum).SingleOrDefault();
                var YearID3 = db.FnFinYears.Where(x => x.WebsiteID == MyWebsiteID && x.IsCurrentYear2 == true && x.IsTrash == false).Select(x => x.YearNum).SingleOrDefault();
                var YearIDInv = db.FnFinYears.Where(x => x.WebsiteID == MyWebsiteID && x.IsCurrentYear2 == true && x.IsTrash == false).Select(x => x.AutoID).SingleOrDefault();

                string _YearNum = "";
                if ((TypeID == 5) || (TypeID == 26))
                {
                    _YearNum = (YearID2.HasValue) ? YearID2.ToString() : "";
                }
                else
                {
                    _YearNum = (YearID3.HasValue) ? YearID3.ToString() : "";
                }
  
                string Tag2 = MyWebsiteID.ToString() + "" + _YearNum.Substring(_YearNum.Length - 2);



                if (TypeID == 5)
                {
                    string Tag = "IO";
                    var query = (from ex in db.ScInventoriesOuts where ex.WebsiteID == MyWebsiteID && ex.IsTrash == false && ex.IsActive == true && ex.YearId == YearIDInv orderby ex.AutoID descending select ex.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 6)
                {
                    string Tag = "IN";
                    var query = (from ex in db.ScInventoriesIns where ex.WebsiteID == MyWebsiteID && ex.IsTrash == false && ex.IsActive == true && ex.YearId == YearIDInv orderby ex.AutoID descending select ex.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 9)
                {
                    string Tag = "SI";
                    var query = db.ScSalesInvoices.Where(x => x.WebsiteID == MyWebsiteID && (x.InvoicesStatus == 8 || x.InvoicesStatus == 2) && x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 11)
                {
                    string Tag = "SP";
                    var query = db.ScSalesInvoices.Where(x => x.WebsiteID == MyWebsiteID && x.InvoicesStatus == 2 && x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 12)
                {
                    string Tag = "SR";
                    var query = db.ScSalesInvoices.Where(x => x.WebsiteID == MyWebsiteID && x.InvoicesStatus == 11 && x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 13)
                {
                    string Tag = "SB";
                    var query = db.ScSalesReturns.Where(x => x.WebsiteID == MyWebsiteID && x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 16)
                {
                    string Tag = "RR";
                    var query = db.ScSalesInvoices.Where(x => x.WebsiteID == MyWebsiteID && x.InvoicesStatus == 16 && x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 17)
                {
                    string Tag = "SU";
                    var query = db.ScSalesInvoices.Where(x => x.WebsiteID == MyWebsiteID && x.InvoicesStatus == 17 && x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 18)
                {
                    string Tag = "PO";
                    var query = db.ScSalesInvoices.Where(x => x.WebsiteID == MyWebsiteID && x.InvoicesStatus == 18 && x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 25)
                {
                    string Tag = "PR";
                    var query = db.ScPurchasesRequests.Where(x => x.WebsiteID == MyWebsiteID &&  x.IsTrash == false && x.IsActive == true && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 26)
                {
                    string Tag = "PC";
                    var query = db.ScInventoryProcessesContinues.Where(x => x.WebsiteID == MyWebsiteID && x.IsTrash == false && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 27)
                {
                    string Tag = "TI";
                    var query = db.ScCommandTranferInventories.Where(x => x.WebsiteID == MyWebsiteID && x.IsTrash == false && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 28)
                {
                    string Tag = "DP";
                    var query = db.ScProcessesDamagedStocks.Where(x => x.WebsiteID == MyWebsiteID && x.IsTrash == false && x.YearId == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Name).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                else if (TypeID == 29)
                {
                    string Tag = "RV";
                    var query = db.FnReciptVouchers.Where(x => x.WebsiteID == MyWebsiteID && x.IsTrash == false && x.YearID == YearIDInv).OrderByDescending(x => x.AutoID).Select(x => x.Num).FirstOrDefault();
                    if (query == null)
                    {
                        AutoNum = Tag + Tag2 + 1.ToString("D4");
                    }
                    else
                    {
                        var InvoiceNum = query.Substring(query.Length - 4);

                        AutoNum = Tag + Tag2 + (int.Parse(InvoiceNum) + 1).ToString("D4");
                    }
                }
                return AutoNum;
            }
        }

        public static int ActiveYearID(int TypeID, int MyWebsiteID)
        {
            using (var db = new ShoppingCartConnection())
            {
                string AutoNum = string.Empty;
                var YearID2 = db.FnFinYears.Where(x => x.WebsiteID == MyWebsiteID && x.IsCurrentYear == true && x.IsTrash == false).Select(x => x.AutoID).SingleOrDefault();
                var YearID3 = db.FnFinYears.Where(x => x.WebsiteID == MyWebsiteID && x.IsCurrentYear2 == true && x.IsTrash == false).Select(x => x.AutoID).SingleOrDefault();

                int _YearID = 0;
                if ((TypeID == 5) || (TypeID == 26))
                {
                    _YearID = YearID2;
                }
                else
                {
                    _YearID = YearID3;
                }
                
                return _YearID;
            }
        }

    }
}