using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TpService.Lib;
using TpService.Models;

namespace TpService.Controllers
{
    [AllowCrossSite]
    [RoutePrefix("Gooobig")]
    public class GooobigServiceController : Controller
    {
        [HttpPost]
        [Route("Login")]
        public ActionResult Login(string userName, string password)
        {
            var header = this.Request.Headers;
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) {
                return Json(new { StatusCode = 202 }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var _context = new ShoppingCartConnection())
                {
                    var MngPass = new TpMngPasswordCryption();
                    var encPassword = MngPass.Encrypt(password);

                    var _employee = _context.Employees.Where(x => x.IsActive == true && x.IsTrash == false && x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && x.Password == encPassword);
                    if (_employee.Any())
                    {
                        var emp = _employee.Select(x => new { x.FullName, x.WebsiteID, x.EmployeeID }).Single();
                        var result = new
                        {
                            UserId = emp.EmployeeID,
                            Name = emp.FullName,
                            SiteId = emp.WebsiteID
                        };

                        return Json(new { StatusCode = 200, Info = result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception)
            {
                return Json(new { StatusCode = 500 }, JsonRequestBehavior.AllowGet); ;

            }
        }


        [HttpPost]
        [Route("Register")]
        public ActionResult Register(string firstName, string lastName, string userName, string password, string phone, string email)
        {
            var header = this.Request.Headers;
            if (string.IsNullOrWhiteSpace(firstName) ||
                   string.IsNullOrWhiteSpace(lastName) ||
                   string.IsNullOrWhiteSpace(userName) ||
                   string.IsNullOrWhiteSpace(password) ||
                   header["siteId"] == null)
            {
                return Json(new { StatusCode = 202 }, JsonRequestBehavior.AllowGet);
            }


            try
            {

                if (GooobigHelper.CheckUserName(userName))
                {
                    return Json(new { StatusCode = 201 }, JsonRequestBehavior.AllowGet);
                }
                var _siteId = int.Parse(header["SiteId"]);

                using (var _context = new ShoppingCartConnection())
                {
                    var MngPass = new TpMngPasswordCryption();
                    var encPassword = MngPass.Encrypt(password);


                    var Obj = new Employee();
                    Obj.FirstName = firstName;
                    Obj.ThirdName = lastName;
                    Obj.FamilyName = lastName;
                    Obj.FullName = firstName + " " + lastName;
                    Obj.NickName = Obj.FullName;
                    Obj.SID = "1234567890";
                    Obj.CellPhoneNumber = phone;
                    Obj.UserName = userName;
                    Obj.Email = email;
                    Obj.Password = encPassword;
                    Obj.IsActive = true;
                    Obj.IsTrash = false;
                    Obj.UpdatedDate = DateTime.Now;
                    Obj.CreatedDate = DateTime.Now;
                    Obj.CreatedBy = 100;
                    Obj.UpdatedBy = 100;
                    Obj.WebsiteID = _siteId;
                    _context.Employees.Add(Obj);
                    _context.SaveChanges();

                    var empClient = new ScClient();
                    empClient.Type = "C";
                    empClient.ClientType = 1;
                    empClient.Currency = "S";
                    empClient.DealTypeNow = true;
                    empClient.OrderBy = 1;
                    empClient.EmployeeID = Obj.EmployeeID;
                    empClient.WebsiteID = _siteId;
                    empClient.IsActive = true;
                    empClient.IsTrash = false;
                    empClient.UpdatedDate = DateTime.Now;
                    empClient.CreatedDate = DateTime.Now;
                    empClient.CreatedBy = 100;
                    _context.ScClients.Add(empClient);
                    _context.SaveChanges();

                    var result = new
                    {
                        UserId = Obj.EmployeeID,
                        Name = Obj.FullName,
                        SiteId = Obj.WebsiteID
                    };
                    return Json(new { StatusCode = 200, Info = result }, JsonRequestBehavior.AllowGet);


                }
            }
            catch (Exception)
            {
                return Json(new { StatusCode = 500 }, JsonRequestBehavior.AllowGet); ;

            }
        }



        [HttpPost]
        [Route("getCategories")]
        public ActionResult getCategories(int? categoryId, bool withImage = false)
        {
            var header = this.Request.Headers;
            if (header["siteId"] == null)
            {
                return Json(new { StatusCode = 202 }, JsonRequestBehavior.AllowGet); ;

            }
            var _siteId = int.Parse(header["SiteId"]);
            try
            {
                using (var _context = new ShoppingCartConnection())
                {
                    var _categories = _context.ScCategories.Where(x => x.WebsiteID == _siteId && x.IsActive == true && x.IsTrash == false).ToList();

                    if (categoryId != null)
                    {
                        var _category = _categories.Where(x => x.AutoID == categoryId).SingleOrDefault();
                        if (_category != null)
                        {
                            if (withImage)
                            {
                                var _result = new
                                {
                                    CategoryId = _category.AutoID,
                                    CategoryName = _category.Name,
                                    CategoryParentId = _category.ParentID,
                                    CategoryImageUrl = _category.ImageID.HasValue ? Products.GetImageLink(_category.ImageID) : ""
                                };

                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;
                            }
                            else
                            {
                                var _result = new
                                {
                                    CategoryId = _category.AutoID,
                                    CategoryName = _category.Name,
                                    CategoryParentId = _category.ParentID,
                                };
                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;

                            }
                        }
                        else
                        {
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (_categories != null)
                        {
                            if (withImage)
                            {
                                var _result = _categories.Select(x => new
                                {
                                    CategoryId = x.AutoID,
                                    CategoryName = x.Name,
                                    CategoryParentId = x.ParentID,
                                    CategoryImageUrl = x.ImageID.HasValue ? Products.GetImageLink(x.ImageID) : ""
                                });

                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;
                            }
                            else
                            {
                                var _result = _categories.Select(x => new
                                {
                                    CategoryId = x.AutoID,
                                    CategoryName = x.Name,
                                    CategoryParentId = x.ParentID
                                });
                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;

                            }
                        }
                        else
                        {
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                        }

                    }

                }
            }
            catch (Exception)
            {
                return Json(new { StatusCode = 500 }, JsonRequestBehavior.AllowGet); ;

            }
        }


        [HttpPost]
        [Route("getSections")]
        public ActionResult getSections(int? sectionId, bool withImage = false)
        {
            var header = this.Request.Headers;
            if (header["siteId"] == null)
            {
                return Json(new { StatusCode = 202 }, JsonRequestBehavior.AllowGet); ;

            }
            var _siteId = int.Parse(header["SiteId"]);
            try
            {
                using (var _context = new ShoppingCartConnection())
                {
                    var _sections = _context.ScProducts.Where(x => x.WebsiteID == _siteId && x.IsActive == true && x.IsTrash == false).ToList();
                    var _categories = _context.ScProductsCategories.ToList();

                    if (sectionId != null)
                    {
                        var obj = _sections.Where(x => x.AutoID == sectionId).SingleOrDefault();
                        if (obj != null)
                        {
                            if (withImage)
                            {
                                var _result = new
                                {
                                    ProductsSectionId = obj.AutoID,
                                    ProductsSectionName = obj.Name,
                                    ProductsSectionCategoriesIds = _categories.Where(x => x.ProductID == sectionId).Select(p => p.CategoryID),
                                    ProductsSectionImageUrl = obj.ImageID.HasValue ? Products.GetImageLink(obj.ImageID) : ""
                                };
                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var _result = new
                                {
                                    ProductsSectionId = obj.AutoID,
                                    ProductsSectionName = obj.Name,
                                    ProductsSectionCategoriesIds = _categories.Where(x => x.ProductID == sectionId).Select(p => p.CategoryID),
                                };
                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (_sections != null)
                        {
                            if (withImage)
                            {
                                var _result = _sections.Select(x => new
                                {
                                    ProductsSectionId = x.AutoID,
                                    ProductsSectionName = x.Name,
                                    ProductsSectionCategoriesIds = _categories.Where(p => p.ProductID == x.AutoID).Select(p => p.CategoryID),
                                    ProductsSectionImageUrl = x.ImageID.HasValue ? Products.GetImageLink(x.ImageID) : ""
                                });

                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;
                            }
                            else
                            {
                                var _result = _sections.Select(x => new
                                {
                                    ProductsSectionId = x.AutoID,
                                    ProductsSectionName = x.Name,
                                    ProductsSectionCategoriesIds = _categories.Where(p => p.ProductID == x.AutoID).Select(p => p.CategoryID)
                                });
                                return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;

                            }
                        }
                        else
                        {
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                        }
                    }


                }
            }
            catch (Exception)
            {
                return Json(new { StatusCode = 500 }, JsonRequestBehavior.AllowGet); ;

            }
        }



        [HttpPost]
        [Route("getMeasureTypes")]
        public ActionResult getMeasureTypes(int? measureTypeId)
        {
            var header = this.Request.Headers;
            if (header["siteId"] == null)
            {
                return Json(new { StatusCode = 202 }, JsonRequestBehavior.AllowGet); ;

            }
            var _siteId = int.Parse(header["SiteId"]);
            try
            {
                using (var _context = new ShoppingCartConnection())
                {
                    var _measureTypes = _context.ClsMeasureTypes.Where(x => x.WebsiteID == _siteId && x.IsActive == true && x.IsTrash == false).ToList();

                    if (measureTypeId != null)
                    {
                        var _measureType = _measureTypes.Where(x => x.AutoID == measureTypeId).SingleOrDefault();
                        if (_measureType != null)
                        {
                            var _result = new
                            {
                                MeasureTypeId = _measureType.AutoID,
                                MeasureTypeName = _measureType.Name,
                                MeasureTypeCode = _measureType.Code,
                            };
                            return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;
                        }
                        else
                        {
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (_measureTypes != null)
                        {
                            var _result = _measureTypes.Select(x => new
                            {
                                MeasureTypeId = x.AutoID,
                                MeasureTypeName = x.Name,
                                MeasureTypeCode = x.Code
                            });
                            return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet); ;

                        }
                        else
                        {
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                        }

                    }

                }
            }
            catch (Exception)
            {
                return Json(new { StatusCode = 500 }, JsonRequestBehavior.AllowGet); ;

            }
        }




        [HttpPost]
        [Route("getProducts")]
        public ActionResult getProducts(int? productId, bool fullInfo = false)
        {
            try
            {
                var header = this.Request.Headers;
                if (header["siteId"] == null)
                    return Json(new { StatusCode = 202 }, JsonRequestBehavior.AllowGet); ;

                var _siteId = int.Parse(header["SiteId"]);

                if (fullInfo)
                {
                    if (productId.HasValue)
                    {
                        var _result = GooobigHelper.getFullProduct(productId.Value);
                        if (_result != null)
                            return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        var _result = GooobigHelper.getFullProductsList(_siteId);
                        if (_result != null)
                            return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (productId.HasValue)
                    {
                        var _result = GooobigHelper.getMiniProduct(productId.Value);
                        if (_result != null)
                            return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var _result = GooobigHelper.getMiniProductsList(_siteId);
                        if (_result != null)
                            return Json(new { StatusCode = 200, Info = _result }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { StatusCode = 404 }, JsonRequestBehavior.AllowGet);
                    }
                }


            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet); ;

            }
        }





    }

}
