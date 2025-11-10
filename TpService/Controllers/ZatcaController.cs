using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using TpService.Lib;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TpService.Models;
using TpService.Models.Data;
using TpService.Models.ViewModels;
using TpService.Models.ViewModels.PrepareScreen;
using ZatcaIntegrationSDK;
using ZatcaIntegrationSDK.BLL;
using ZatcaIntegrationSDK.HelperContracts;
using ZXing;
using ZXing.Common;
using System.Xml;

namespace TpService.Controllers
{
    [AllowCrossSite]
    [RoutePrefix("Zatca")]
    public class ZatcaController : Controller
    {
        [HttpGet]
        [Route("CheckPostMan")]
        public ActionResult CheckPostMan()
        {

            return Json(new { StatusCode = 200, msg = "Hi" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [Route("getInvoices")]
        public ActionResult getInvoices()
        {
            var header = this.Request.Headers;
            if (header["auth_key"] == null)
            {
                return Json(new { StatusCode = 400, StatusMessage = "Please provide auth_key" }, JsonRequestBehavior.AllowGet);

            }
            var _auth_key = header["auth_key"];
            var latestUpdate = Convert.ToDateTime(header["latestUpdate"]);
            var _userInformation = Authentication(_auth_key);
            if (!_userInformation.IsAuthenticated)
            {
                return Json(new { StatusCode = 401, StatusMessage = "Please provide Valid Autheztication Key" }, JsonRequestBehavior.AllowGet);
            }
            var _siteId = _userInformation.UserLoginInfo.WebsiteID;
            try
            {
                using (var _context = new ShoppingCartConnection())
                {
                    var _MySalesList = _context.ScSalesInvoices.Where(x => x.IsActive == true && x.IsTrash == false)
                       .Select(x => new { x.AutoID, x.Name, x.Details, x.WebsiteID }).ToList();

                    List<ScSalesInvoice> SalesList = new List<ScSalesInvoice>();
                    foreach (var item in _MySalesList)
                    {
                        ScSalesInvoice ProductInfo = new ScSalesInvoice();
                        ProductInfo.AutoID = item.AutoID;
                        ProductInfo.Name = item.Name;
                        ProductInfo.Details = item.Details;
                        ProductInfo.WebsiteID = item.WebsiteID;
                        SalesList.Add(ProductInfo);

                    }
                    return Json(new { StatusCode = 200, Info = SalesList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [Route("getInvoiceDetails")] //   إحضار بيانات فاتورة ------------------------------- >
        [HttpGet]
        public ActionResult getInvoiceDetails(int invId)
        {
            var header = this.Request.Headers;

            if (header["auth_key"] == null)
            {
                return Json(new { StatusCode = 400, StatusMessage = "Please provide auth_key" }, JsonRequestBehavior.AllowGet);

            }
            var _auth_key = header["auth_key"];
            var _userInformation = Authentication(_auth_key);
            if (!_userInformation.IsAuthenticated)
            {
                return Json(new { StatusCode = 401, StatusMessage = "Please provide Valid Autheztication Key" }, JsonRequestBehavior.AllowGet);
            }
            var _siteId = _userInformation.UserLoginInfo.WebsiteID.Value;
            try
            {
                using (var db = new ShoppingCartConnection())
                {
                    try
                    {
                        var _invoiceData = db.ScSalesInvoices.Where(x => x.AutoID == invId).FirstOrDefault();
                        var Products = db.ScSubSalesInvoices.Where(x => x.InvoiceID == _invoiceData.AutoID).ToList();
                        var Payments = db.ScSalesInvoicePaymentsTypes.Where(x => x.InvoiceID == invId).ToList();
                        var Services = db.ScSubSalesExtraFees.Where(x => x.InvoiceID == invId).ToList();
                        var Taxes = db.ScSubSalesTaxes.Where(x => x.InvoiceID == invId).ToList();

                        return Json(new
                        {
                            statusCode = 200,
                            data = new
                            {
                                _invoiceData,
                                Products,
                                Payments,
                                Services,
                                Taxes
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { StatusCode = 401, msg = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //compliance
        [HttpPost]
        [Route("ComplianceCSID")]
        public ActionResult ComplianceCSID(RequestCSIDModel info)
        {
            var ss = new JavaScriptSerializer().Serialize(info);

            var header = this.Request.Headers;
            if (header["auth_key"] == null)
            {
                return Json(new { StatusCode = 400, StatusMessage = "Please provide auth_key" }, JsonRequestBehavior.AllowGet);

            }
            var _auth_key = header["auth_key"];
            var _userInformation = Authentication(_auth_key);
            if (!_userInformation.IsAuthenticated)
            {
                return Json(new { StatusCode = 401, StatusMessage = "Please provide Valid Autheztication Key" }, JsonRequestBehavior.AllowGet);
            }
            var _siteId = _userInformation.UserLoginInfo.WebsiteID;
            using (var db = new ShoppingCartConnection())
            {
                ComplianceCsrResponse _result = new ComplianceCsrResponse();

                try
                {
                    int _DeviceID = Convert.ToInt32(info.DeviceID);

                    var InvInfo = db.CgMngDevicesZatcas.Where(x => x.DeviceID == _DeviceID && x.IsActive == true && x.IsTrash == false).SingleOrDefault();

                    if (InvInfo == null)
                    {
                        complianceCSIDModel _Info = new complianceCSIDModel();
                        _Info.csr = info.CSR;
                        string mainUri = GetZatcaApiLink(Mode.developer);
                        long? OTP = info.OTP;

                        HttpClient cons = new HttpClient();
                        cons.BaseAddress = new Uri(mainUri);
                        cons.DefaultRequestHeaders.Accept.Clear();
                        cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        cons.DefaultRequestHeaders.Add("OTP", OTP.ToString());
                        cons.DefaultRequestHeaders.Add("Accept-Version", "V2");

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | SecurityProtocolType.Tls;

                        var content = new StringContent(JsonConvert.SerializeObject(new { csr = _Info.csr }), Encoding.UTF8, "application/json");

                        string partUri = "compliance";
                        HttpResponseMessage responsePost = cons.PostAsync(partUri, content).Result;
                        var reponsestr = responsePost.Content.ReadAsStringAsync().Result;
                        _result.StatusCode = (int)responsePost.StatusCode;
                        if (_result.StatusCode == 200)
                        {
                            _result = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);

                            var obj = new CgMngDevicesZatca();
                            obj.DeviceID = Convert.ToInt32(info.DeviceID);
                            obj.OTP = info.OTP;
                            obj.PrivateKey = info.PrivateKey;
                            obj.PublicKey = info.PublicKey;
                            obj.CSR = info.CSR;
                            obj.requestID = Convert.ToInt64(_result.RequestId);
                            obj.dispositionMessage = _result.DispositionMessage;
                            obj.binarySecurityToken = _result.BinarySecurityToken;
                            obj.secret = _result.Secret;
                            obj.IsActive = true;
                            obj.IsTrash = false;
                            obj.CreatedBy = _userInformation.UserLoginInfo.EmployeeID;
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedBy = _userInformation.UserLoginInfo.EmployeeID;
                            obj.UpdatedDate = DateTime.Now;
                            obj.WebsiteID = _siteId;
                            db.CgMngDevicesZatcas.Add(obj);
                            db.SaveChanges();

                            return Json(new { StatusCode = 200, result = _result, msg = "ok" }, JsonRequestBehavior.AllowGet);
                        }
                        _result.ErrorMessage = "Error StatusCode : " + responsePost.StatusCode + "  \n\r";
                        if (responsePost.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ComplianceCsrResponse error = new ComplianceCsrResponse();
                            try
                            {
                                error = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);
                                _result.ErrorMessage += error.DispositionMessage + " : " + string.Join("\n", error.Errors);
                            }
                            catch
                            {
                                _result.ErrorMessage += reponsestr;
                            }
                        }
                        else if (responsePost.StatusCode == HttpStatusCode.NotAcceptable)
                        {

                            _result.ErrorMessage += "This Version is not supported or not provided in the header.";

                        }
                        else if (responsePost.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ErrorModel error = new ErrorModel();
                            try
                            {
                                error = JsonConvert.DeserializeObject<ErrorModel>(reponsestr);
                                _result.ErrorMessage += error.Code + " : " + error.Message;
                            }
                            catch
                            {
                                _result.ErrorMessage += reponsestr;
                            }
                        }
                        else
                        {
                            _result.ErrorMessage += "Error in ComplianceCsr API";
                        }
                        return Json(new { StatusCode = 200, result = _result, msg = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { StatusCode = 201, msg = "already" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { StatusCode = 400, result = _result, msg = ex.Message }, JsonRequestBehavior.AllowGet);
                }

            }

        }

        //production/csids
        [HttpPost]
        [Route("ProductionCSIDOnBoarding")]
        public ActionResult ProductionCSIDOnBoarding(RequestCSIDModel info)
        {
            var ss = new JavaScriptSerializer().Serialize(info);

            var header = this.Request.Headers;
            if (header["auth_key"] == null)
            {
                return Json(new { StatusCode = 400, StatusMessage = "Please provide auth_key" }, JsonRequestBehavior.AllowGet);

            }
            var _auth_key = header["auth_key"];
            var _userInformation = Authentication(_auth_key);
            if (!_userInformation.IsAuthenticated)
            {
                return Json(new { StatusCode = 401, StatusMessage = "Please provide Valid Autheztication Key" }, JsonRequestBehavior.AllowGet);
            }
            var _siteId = _userInformation.UserLoginInfo.WebsiteID;
            using (var db = new ShoppingCartConnection())
            {
                ComplianceCsrResponse _result = new ComplianceCsrResponse();

                try
                {
                    int _DeviceID = Convert.ToInt32(info.DeviceID);

                    var InvInfo = db.CgMngDevicesZatcas.Where(x => x.DeviceID == _DeviceID && x.IsActive == true && x.IsTrash == false).SingleOrDefault();

                    if (InvInfo == null)
                    {
                        complianceCSIDModel _Info = new complianceCSIDModel();
                        _Info.csr = info.CSR;
                        string mainUri = GetZatcaApiLink(Mode.developer);

                        HttpClient cons = new HttpClient();
                        cons.BaseAddress = new Uri(mainUri);
                        cons.DefaultRequestHeaders.Accept.Clear();
                        cons.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{InvInfo.binarySecurityToken}:{InvInfo.secret}")));
                        cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        cons.DefaultRequestHeaders.Add("Accept-Version", "V2");

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | SecurityProtocolType.Tls;

                        var content = new StringContent(JsonConvert.SerializeObject(new { compliance_request_id = InvInfo.requestID }), Encoding.UTF8, "application/json");

                        string partUri = "production/csids";
                        HttpResponseMessage responsePost = cons.PostAsync(partUri, content).Result;
                        var reponsestr = responsePost.Content.ReadAsStringAsync().Result;
                        _result.StatusCode = (int)responsePost.StatusCode;
                        if (_result.StatusCode == 200)
                        {
                            _result = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);

                            var obj = new CgMngDevicesZatca();
                            obj.DeviceID = Convert.ToInt32(info.DeviceID);
                            obj.OTP = info.OTP;
                            obj.PrivateKey = info.PrivateKey;
                            obj.PublicKey = info.PublicKey;
                            obj.CSR = info.CSR;
                            obj.requestID = Convert.ToInt64(_result.RequestId);
                            obj.dispositionMessage = _result.DispositionMessage;
                            obj.binarySecurityToken = _result.BinarySecurityToken;
                            obj.secret = _result.Secret;
                            obj.IsActive = true;
                            obj.IsTrash = false;
                            obj.CreatedBy = _userInformation.UserLoginInfo.EmployeeID;
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedBy = _userInformation.UserLoginInfo.EmployeeID;
                            obj.UpdatedDate = DateTime.Now;
                            obj.WebsiteID = _siteId;
                            db.CgMngDevicesZatcas.Add(obj);
                            db.SaveChanges();

                            return Json(new { StatusCode = 200, result = _result, msg = "ok" }, JsonRequestBehavior.AllowGet);
                        }
                        _result.ErrorMessage = "Error StatusCode : " + responsePost.StatusCode + "  \n\r";
                        if (responsePost.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ComplianceCsrResponse error = new ComplianceCsrResponse();
                            try
                            {
                                error = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);
                                _result.ErrorMessage += error.DispositionMessage + " : " + string.Join("\n", error.Errors);
                            }
                            catch
                            {
                                _result.ErrorMessage += reponsestr;
                            }
                        }
                        else if (responsePost.StatusCode == HttpStatusCode.NotAcceptable)
                        {

                            _result.ErrorMessage += "This Version is not supported or not provided in the header.";

                        }
                        else if (responsePost.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ErrorModel error = new ErrorModel();
                            try
                            {
                                error = JsonConvert.DeserializeObject<ErrorModel>(reponsestr);
                                _result.ErrorMessage += error.Code + " : " + error.Message;
                            }
                            catch
                            {
                                _result.ErrorMessage += reponsestr;
                            }
                        }
                        else
                        {
                            _result.ErrorMessage += "Error in ComplianceCsr API";
                        }
                        return Json(new { StatusCode = 200, result = _result, msg = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { StatusCode = 201, msg = "already" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { StatusCode = 400, result = _result, msg = ex.Message }, JsonRequestBehavior.AllowGet);
                }

            }

        }

        //production/csids
        [HttpPost]
        [Route("ProductionCSIDRenewal")]
        public ActionResult ProductionCSIDRenewal(RequestCSIDModel info)
        {
            var ss = new JavaScriptSerializer().Serialize(info);

            var header = this.Request.Headers;
            if (header["auth_key"] == null)
            {
                return Json(new { StatusCode = 400, StatusMessage = "Please provide auth_key" }, JsonRequestBehavior.AllowGet);

            }
            var _auth_key = header["auth_key"];
            var _userInformation = Authentication(_auth_key);
            if (!_userInformation.IsAuthenticated)
            {
                return Json(new { StatusCode = 401, StatusMessage = "Please provide Valid Autheztication Key" }, JsonRequestBehavior.AllowGet);
            }
            var _siteId = _userInformation.UserLoginInfo.WebsiteID;
            using (var db = new ShoppingCartConnection())
            {
                ComplianceCsrResponse _result = new ComplianceCsrResponse();

                try
                {
                    int _DeviceID = Convert.ToInt32(info.DeviceID);

                    var InvInfo = db.CgMngDevicesZatcas.Where(x => x.DeviceID == _DeviceID && x.IsActive == true && x.IsTrash == false).SingleOrDefault();

                    if (InvInfo == null)
                    {
                        complianceCSIDModel _Info = new complianceCSIDModel();
                        _Info.csr = info.CSR;
                        string mainUri = GetZatcaApiLink(Mode.developer);
                        long? OTP = info.OTP;

                        HttpClient cons = new HttpClient();
                        cons.BaseAddress = new Uri(mainUri);
                        cons.DefaultRequestHeaders.Accept.Clear();
                        cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        cons.DefaultRequestHeaders.Add("OTP", OTP.ToString());
                        cons.DefaultRequestHeaders.Add("accept-language", "en");
                        cons.DefaultRequestHeaders.Add("Accept-Version", "V2");

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | SecurityProtocolType.Tls;

                        var content = new StringContent(JsonConvert.SerializeObject(new { csr = _Info.csr }), Encoding.UTF8, "application/json");

                        string partUri = "production/csids";

                        var request = new HttpRequestMessage(new HttpMethod("PATCH"), cons.BaseAddress.ToString() + partUri);
                        //var request = new HttpRequestMessage(new HttpMethod("PATCH"), partUri);

                        request.Content = content;
                        HttpResponseMessage responsePost = cons.SendAsync(request).Result;

                        var reponsestr = responsePost.Content.ReadAsStringAsync().Result;

                        _result.StatusCode = (int)responsePost.StatusCode;
                        if (_result.StatusCode == 200)
                        {
                            _result = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);

                            var obj = new CgMngDevicesZatca();
                            obj.DeviceID = Convert.ToInt32(info.DeviceID);
                            obj.OTP = info.OTP;
                            obj.PrivateKey = info.PrivateKey;
                            obj.PublicKey = info.PublicKey;
                            obj.CSR = info.CSR;
                            obj.requestID = Convert.ToInt64(_result.RequestId);
                            obj.dispositionMessage = _result.DispositionMessage;
                            obj.binarySecurityToken = _result.BinarySecurityToken;
                            obj.secret = _result.Secret;
                            obj.IsActive = true;
                            obj.IsTrash = false;
                            obj.CreatedBy = _userInformation.UserLoginInfo.EmployeeID;
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedBy = _userInformation.UserLoginInfo.EmployeeID;
                            obj.UpdatedDate = DateTime.Now;
                            obj.WebsiteID = _siteId;
                            db.CgMngDevicesZatcas.Add(obj);
                            db.SaveChanges();

                            return Json(new { StatusCode = 200, result = _result, msg = "ok" }, JsonRequestBehavior.AllowGet);
                        }
                        _result.ErrorMessage = "Error StatusCode : " + responsePost.StatusCode + "  \n\r";
                        if (responsePost.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ComplianceCsrResponse error = new ComplianceCsrResponse();
                            try
                            {
                                error = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);
                                _result.ErrorMessage += error.DispositionMessage + " : " + string.Join("\n", error.Errors);
                            }
                            catch
                            {
                                _result.ErrorMessage += reponsestr;
                            }
                        }
                        else if (responsePost.StatusCode == HttpStatusCode.NotAcceptable)
                        {

                            _result.ErrorMessage += "This Version is not supported or not provided in the header.";

                        }
                        else if (responsePost.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ErrorModel error = new ErrorModel();
                            try
                            {
                                error = JsonConvert.DeserializeObject<ErrorModel>(reponsestr);
                                _result.ErrorMessage += error.Code + " : " + error.Message;
                            }
                            catch
                            {
                                _result.ErrorMessage += reponsestr;
                            }
                        }
                        else
                        {
                            _result.ErrorMessage += "Error in ComplianceCsr API";
                        }
                        return Json(new { StatusCode = 200, result = _result, msg = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { StatusCode = 201, msg = "already" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { StatusCode = 400, result = _result, msg = ex.Message }, JsonRequestBehavior.AllowGet);
                }

            }

        }

        //compliance/invoices
        [HttpPost]
        [Route("ComplianceInvoices")] //فحص الفواتير --------------------------------------------------- >
        public ActionResult ComplianceInvoices(RequestCSIDModel info, int invoiceid)
        {
            ComplianceCsrResponse _result = new ComplianceCsrResponse();
            try
            {
                using (var db = new ShoppingCartConnection())
                {
                    int _DeviceID = Convert.ToInt32(info.DeviceID);

                    var InvInfo = db.CgMngDevicesZatcas.Where(x => x.DeviceID == _DeviceID && x.IsActive == true && x.IsTrash == false).SingleOrDefault();

                    if (InvInfo == null)
                    {
                        UBLXML ubl = new UBLXML();
                        ZatcaIntegrationSDK.Result res = new ZatcaIntegrationSDK.Result();

                        var _invoice = new Invoice();
                        _invoice = ZatcaSalesInvoice(invoiceid, 2); // 1 standard 2 simplified
                        bool savexmlfile = true;
                        res = ubl.GenerateInvoiceXML(_invoice, Directory.GetCurrentDirectory(), savexmlfile);

                        InvoiceReportingRequest invrequestbody = new InvoiceReportingRequest();
                        invrequestbody.invoice = res.EncodedInvoice;
                        invrequestbody.invoiceHash = res.InvoiceHash;
                        invrequestbody.uuid = res.UUID;
                        //Image PictureBox1 = QrCodeImage(res.QRCode, 200, 200);

                        string mainUri = GetZatcaApiLink(Mode.developer);

                        HttpClient cons = new HttpClient();
                        cons.BaseAddress = new Uri(mainUri);
                        cons.DefaultRequestHeaders.Accept.Clear();
                        cons.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{InvInfo.binarySecurityToken}:{InvInfo.secret}")));
                        cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        cons.DefaultRequestHeaders.Add("Accept-Version", "V2");
                        cons.DefaultRequestHeaders.Add("accept-language", "en");

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | SecurityProtocolType.Tls;

                        var content = new StringContent(JsonConvert.SerializeObject(new { invrequestbody }), Encoding.UTF8, "application/json");

                        string partUri = "compliance/invoices";
                        HttpResponseMessage responsePost = cons.PostAsync(partUri, content).Result;
                        var reponsestr = responsePost.Content.ReadAsStringAsync().Result;
                        _result.StatusCode = (int)responsePost.StatusCode;
                        if (_result.StatusCode == 200)
                        {
                            return Json(new { StatusCode = 200, result = _result }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { StatusCode = 201, result = _result }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { StatusCode = 202, result = _result }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 400, result = _result, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //compliance/invoices
        [HttpPost]
        [Route("TestInvoices")] //فحص الفواتير --------------------------------------------------- >
        public ActionResult TestInvoices(int invoiceid)
        {
            try
            {
                using (var db = new ShoppingCartConnection())
                {

                    var _invoice = new Invoice();
                    _invoice = ZatcaSalesInvoice(invoiceid, 2); // 1 standard 2 simplified

                    if (_invoice != null)
                    {
                        return Json(new { StatusCode = 200, result = _invoice }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { StatusCode = 201, result = _invoice }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 400, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //compliance/invoices
        [HttpPost]
        [Route("TestInvoices2")] //فحص الفواتير --------------------------------------------------- >
        public ActionResult TestInvoices2(int invoiceid)
        {
            ComplianceCsrResponse _result = new ComplianceCsrResponse();
            try
            {
                using (var db = new ShoppingCartConnection())
                {
                    var InvoiceInfo = db.ScSalesInvoices.Where(x => x.AutoID == invoiceid).SingleOrDefault();

                    var InvInfo = db.ScInventories.Where(x => x.AutoID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();
                    //temp// var DeviceInfo = db.CgDevicesInfoes.Where(x => x.InventoryID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();
                    //var DeviceInfo = db.CgDevicesInfoes.Where(x => x.InventoryID == InvoiceInfo.WarehouseID && x.AutoID == 2663 && x.IsActive == true && x.IsTrash == false).FirstOrDefault();

                    //var CSIDInfo = db.CgMngDevicesZatcas.Where(x => x.DeviceID == DeviceInfo.AutoID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();


                    var CSIDInfo = db.CgMngDevicesZatcas.Where(x => x.IsActive == true && x.IsTrash == false).FirstOrDefault();


                    if (CSIDInfo != null)
                    {
                        UBLXML ubl = new UBLXML();
                        ZatcaIntegrationSDK.Result res = new ZatcaIntegrationSDK.Result();

                        var _invoice = new Invoice();
                        _invoice = ZatcaSalesInvoice(invoiceid, 2); // 1 standard 2 simplified
                        bool savexmlfile = true;
                        res = ubl.GenerateInvoiceXML(_invoice, Directory.GetCurrentDirectory(), savexmlfile);

                        InvoiceReportingRequest invrequestbody = new InvoiceReportingRequest();
                        invrequestbody.invoice = res.EncodedInvoice;
                        invrequestbody.invoiceHash = res.InvoiceHash;
                        invrequestbody.uuid = res.UUID;
                        //Image PictureBox1 = QrCodeImage(res.QRCode, 200, 200);

                        string mainUri = GetZatcaApiLink(Mode.developer);

                        HttpClient cons = new HttpClient();
                        cons.BaseAddress = new Uri(mainUri);
                        cons.DefaultRequestHeaders.Accept.Clear();
                        cons.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{CSIDInfo.binarySecurityToken}:{CSIDInfo.secret}")));
                        cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        cons.DefaultRequestHeaders.Add("Accept-Version", "V2");
                        cons.DefaultRequestHeaders.Add("accept-language", "en");

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | SecurityProtocolType.Tls;

                        var content = new StringContent(JsonConvert.SerializeObject(new { invrequestbody }), Encoding.UTF8, "application/json");

                        string partUri = "compliance/invoices";
                        HttpResponseMessage responsePost = cons.PostAsync(partUri, content).Result;
                        var reponsestr = responsePost.Content.ReadAsStringAsync().Result;
                        _result.StatusCode = (int)responsePost.StatusCode;
                        if (_result.StatusCode == 200)
                        {
                            return Json(new { StatusCode = 200, result = _result }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { StatusCode = 201, result = _result }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { StatusCode = 202, result = _result }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    StatusCode = 400,
                    result = _result,
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [NonAction]
        public Bitmap QrCodeImage(string Qrcode, int width = 250, int height = 250)
        {

            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height
                }
            };
            Bitmap QrCode = barcodeWriter.Write(Qrcode);

            return QrCode;
        }


        [NonAction]
        public Invoice ZatcaSalesInvoice(int InvoiceID, int InvoiceType)
        {
            try
            {
                using (var db = new ShoppingCartConnection())
                {
                    var InvoiceInfo = db.ScSalesInvoices.Where(x => x.AutoID == InvoiceID).SingleOrDefault();
                    if (InvoiceInfo != null)
                    {
                        Invoice _invoice = new Invoice();
                        List<int?> PaymentCreditList = new List<int?>() { 4, 3, 2, 6, 7 };
                        List<int?> PaymentOtherList = new List<int?>() { 8, 9, 5 };
                        List<int?> PaymentTransferList = new List<int?>() { 11, 12, 13 };
                        decimal? invoicetotal = 0;

                        var InvoiceProductList = db.ScSubSalesInvoices.Where(x => x.InvoiceID == InvoiceInfo.AutoID).ToList();
                        var InvoicePaymentList = db.ScSalesInvoicePaymentsTypes.Where(x => x.InvoiceID == InvoiceInfo.AutoID).ToList();

                        var InvInfo = db.ScInventories.Where(x => x.AutoID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();
                        //temp// var DeviceInfo = db.CgDevicesInfoes.Where(x => x.InventoryID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();
                        ////var DeviceInfo = db.CgDevicesInfoes.Where(x => x.InventoryID == InvoiceInfo.WarehouseID && x.AutoID == 2663 && x.IsActive == true && x.IsTrash == false).FirstOrDefault();

                        //var CSIDInfo = db.CgMngDevicesZatcas.Where(x => x.DeviceID == DeviceInfo.AutoID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();
                        var CSIDInfo = db.CgMngDevicesZatcas.Where(x => x.IsActive == true && x.IsTrash == false).FirstOrDefault();

                        var WebsiteInfo = db.MyPortalWebsites.Where(x => x.AutoID == InvInfo.WebsiteID).SingleOrDefault();
                        var WebsiteIdentify = db.MyPortalWebsiteIdentifications.Where(x => x.WebsiteId == InvInfo.WebsiteID && (x.TypeId == 1 || x.TypeId == 2)).ToList();
                        var WebsiteIdentifyCRN = WebsiteIdentify.FirstOrDefault(x => x.TypeId == 1);// db.MyPortalWebsiteIdentifications.Where(x => x.WebsiteId == InvInfo.WebsiteID && x.TypeId == 1).FirstOrDefault();
                        var WebsiteIdentifyTAX = WebsiteIdentify.FirstOrDefault(x => x.TypeId == 2); // db.MyPortalWebsiteIdentifications.Where(x => x.WebsiteId == InvInfo.WebsiteID && x.TypeId == 2).FirstOrDefault();
                                                                                                     // var SupplierPartyInfo = new Invoice();


                        _invoice.cSIDInfo.CertPem = CSIDInfo.CSR;
                        _invoice.cSIDInfo.PrivateKey = CSIDInfo.PrivateKey;

                        // Mandatory /// Optional

                        _invoice.ProfileID = "reporting:1.0"; // 5 // reporting:1.0 
                        _invoice.ID = InvoiceInfo.Name; // 6 // 2021/02/12/1230 
                        _invoice.UUID = Guid.NewGuid().ToString(); // 7 // 3cf5ee18-ee25-44ea-a444-2c37ba7f28be 
                        _invoice.IssueDate = InvoiceInfo.InvoicesDate.ToString("yyyy-MM-dd"); // 8 // 2021-02-25 
                        _invoice.IssueTime = InvoiceInfo.InvoicesDate.TimeOfDay.ToString("hh\\:mm\\:ss\\.ffffff"); // 9 // 16:55:24 
                        _invoice.invoiceTypeCode.id = InvoiceType == 1 || InvoiceType == 2 ? 388 : InvoiceType == 3 ? 383 : InvoiceType == 4 ? 381 : 0; // 10 // 1 standard 388 , 2 simplified 388 , 3 debit 383 , 4 credit 381 
                        _invoice.invoiceTypeCode.Name = InvoiceType == 1 ? "0100000" : "0200000"; // 11 // 1 standard 0100000 , 2 simplified 0200000 
                        ///_invoice.Note = InvoiceInfo.Details; /// 12
                        _invoice.DocumentCurrencyCode = "SAR"; // 13
                        _invoice.TaxCurrencyCode = "SAR"; // 14
                        ///_invoice.orderReference.ID = "" /// 15

                        // بيانات الفاتورة التي عمل عليها مردود
                        if (InvoiceType == 3 || InvoiceType == 4)
                        {
                            InvoiceDocumentReference invoiceDocumentReference = new InvoiceDocumentReference();
                            invoiceDocumentReference.ID = "Invoice Number: 354; Invoice Issue Date: 2021-02-10"; // 16
                            _invoice.billingReference.invoiceDocumentReferences.Add(invoiceDocumentReference);
                        }

                        /// بيانات العقد إذا موجود
                        ///ContractDocumentReference contractDocumentReference = new ContractDocumentReference();
                        ///contractDocumentReference.ID = Guid.NewGuid().ToString(); /// 17 // 161313031
                        ///_invoice.contractDocumentReference = contractDocumentReference;

                        //  رقم عداد الفاتورة ICV
                        AdditionalDocumentReference additionalDocumentReferenceICV = new AdditionalDocumentReference();
                        additionalDocumentReferenceICV.ID = "ICV"; // 17 // ICV:Invoice counter value , PIH:Previous invoice hash , QR:QR code
                        additionalDocumentReferenceICV.UUID = Guid.NewGuid(); // 18 // 46531
                        _invoice.AdditionalDocumentReferenceICV = additionalDocumentReferenceICV;

                        // إذا ماكانت أول فاتورة ترفع
                        // هاش آخر فاتورة تم رفعها PIH
                        // if ScSalesInvoicesZatcaPIH has cleared invoice get last one ////
                        AdditionalDocumentReference additionalDocumentReferencePIH = new AdditionalDocumentReference();
                        additionalDocumentReferencePIH.ID = "PIH"; // 17 // ICV:Invoice counter value , PIH:Previous invoice hash , QR:QR code
                        additionalDocumentReferencePIH.EmbeddedDocumentBinaryObject = string.Empty; // 19 // AQkxMjM0NTY3ODkCCjEyLzEyLzIwMjADBDEwMDADAzE1MPaIn2Z2jg6VqWvWV6IrZZNzLF7xvZrWXW5xRV5yFY2xFu0ycXOiyqV0k7Wsh6b1IcE2Tfzap1AQAQVsktmsv1FFQ1MxIAAAAGKblFMh9nFRSn8tvftXqo9zRSz2VEAPITSZ3W7UDHKhUx+7yXGijLtJSZGXMOc+jpKwARzDl68GmmRd75NWdOs=
                        _invoice.AdditionalDocumentReferenceICV = additionalDocumentReferencePIH;

                        // كيو ار آخر فاتورة تم رفعها PIH
                        AdditionalDocumentReference additionalDocumentReferenceQR = new AdditionalDocumentReference();
                        additionalDocumentReferenceQR.ID = "QR"; // 17 // ICV:Invoice counter value , PIH:Previous invoice hash , QR:QR code
                        additionalDocumentReferenceQR.EmbeddedDocumentBinaryObject = string.Empty; // 19 // AQkxMjM0NTY3ODkCCjEyLzEyLzIwMjADBDEwMDADAzE1MPaIn2Z2jg6VqWvWV6IrZZNzLF7xvZrWXW5xRV5yFY2xFu0ycXOiyqV0k7Wsh6b1IcE2Tfzap1AQAQVsktmsv1FFQ1MxIAAAAGKblFMh9nFRSn8tvftXqo9zRSz2VEAPITSZ3W7UDHKhUx+7yXGijLtJSZGXMOc+jpKwARzDl68GmmRd75NWdOs=
                        _invoice.AdditionalDocumentReferenceICV = additionalDocumentReferenceQR;

                        // بيانات البائع 
                        AccountingSupplierParty supplierParty = new AccountingSupplierParty();
                        supplierParty.partyIdentification.ID = WebsiteIdentifyCRN?.Id; // 16 //"2050012095"; //هنا رقم السجل التجارى للشركة
                        supplierParty.postalAddress.StreetName = WebsiteInfo.StreetName; // 17 // "شارع تجربة"; // اجبارى
                        supplierParty.postalAddress.AdditionalStreetName = WebsiteInfo.AdditionalStreetName; // 18 // "شارع اضافى"; // اختيارى
                        supplierParty.postalAddress.BuildingNumber = WebsiteInfo.BuildingNumber; // 19 // "1234"; // اجبارى رقم المبنى
                        ///supplierParty.postalAddress.PlotIdentification = WebsiteInfo.PlotIdentification.ToString(); // 20 // "9833";
                        supplierParty.postalAddress.CityName = WebsiteInfo.City; // 21 // "taif";
                        supplierParty.postalAddress.PostalZone = WebsiteInfo.PostalZone; // 22 // "12345"; // الرقم البريدي
                        ///supplierParty.postalAddress.CountrySubentity = WebsiteInfo.CountrySubentity; /// 23 // Riyadh Region
                        supplierParty.postalAddress.CitySubdivisionName = WebsiteInfo.CitySubdivisionName; // 24 // "اسم المنطقة"; // اسم المنطقة او الحى 
                        supplierParty.postalAddress.country.IdentificationCode = WebsiteIdentifyCRN?.Code; // 25 // "SA";
                        supplierParty.partyTaxScheme.CompanyID = WebsiteIdentifyTAX?.Id; // 26 // "3xxxxxxxxx00003";  // رقم التسجيل الضريبي
                        supplierParty.partyLegalEntity.RegistrationName = WebsiteInfo.Title; // 27 // "شركة الصناعات الغذائية المتحده"; // اسم الشركة المسجل فى الهيئة
                        supplierParty.partyIdentification.schemeID = WebsiteIdentifyCRN?.SchemeId; // 28 // "CRN";
                        _invoice.SupplierParty = supplierParty;

                        // بيانات المشتري
                        if (InvoiceInfo.SupplierNumber != null)
                        {
                            var ClientInfo = db.ScClients.Where(x => x.AutoID == InvoiceInfo.SupplierNumber).SingleOrDefault();
                            if (ClientInfo != null)
                            {
                                var CityName = db.ClsCities.Where(x => x.AutoID == ClientInfo.City).Select(x => x.Name).SingleOrDefault();
                                if (ClientInfo.AccountNo != null)
                                {
                                    var ClientIdentity = db.ScClientIdentifications.Where(x => x.ClientId == InvoiceInfo.SupplierNumber && (x.TypeId == 1 || x.TypeId == 2)).ToList();
                                    var ClientIdentifyCRN = ClientIdentity.FirstOrDefault(x => x.TypeId == 1);// db.ScClientIdentifications.Where(x => x.ClientId == InvoiceInfo.SupplierNumber && x.TypeId == 1).FirstOrDefault();
                                    var ClientIdentifyTAX = ClientIdentity.FirstOrDefault(x => x.TypeId == 2);// db.ScClientIdentifications.Where(x => x.ClientId == InvoiceInfo.SupplierNumber && x.TypeId == 2).FirstOrDefault();

                                    AccountingCustomerParty customerParty = new AccountingCustomerParty();

                                    customerParty.partyIdentification.ID = ClientIdentifyCRN?.Id; // 28 //"2050012095"; //هنا رقم السجل التجارى للشركة
                                    customerParty.postalAddress.StreetName = ClientInfo.StreetName; // 29 // "شارع تجربة"; // اجبارى
                                    ///customerParty.postalAddress.AdditionalStreetName = ClientInfo.AdditionalStreetName; /// 30 // "شارع اضافى"; // اختيارى
                                    customerParty.postalAddress.BuildingNumber = ClientInfo.BuildingNumber; // 31 // "1234"; // اجبارى رقم المبنى
                                    ///customerParty.postalAddress.PlotIdentification = ClientInfo.PlotIdentification.ToString(); /// 32 // "9833";
                                    customerParty.postalAddress.CityName = CityName; // 33 // "taif";
                                    customerParty.postalAddress.PostalZone = ClientInfo.PostalZone; // 34 // "12345"; // الرقم البريدي
                                    ///customerParty.postalAddress.CountrySubentity = ClientInfo.CountrySubentity; /// 35 // Riyadh Region
                                    customerParty.postalAddress.CitySubdivisionName = ClientInfo.CitySubdivisionName; // 36 // District B
                                    customerParty.postalAddress.country.IdentificationCode = ClientIdentifyCRN?.Code; // 37 // SA
                                    customerParty.partyTaxScheme.CompanyID = ClientIdentifyTAX?.Id; // 38 // 3xxxxxxxxx00003
                                    customerParty.partyIdentification.schemeID = ClientIdentifyCRN?.SchemeId; // "CRN";
                                    customerParty.partyLegalEntity.RegistrationName = ClientInfo.Name; // 39 // "شركة الصناعات الغذائية المتحده"; // اسم الشركة المسجل فى الهيئة
                                    _invoice.CustomerParty = customerParty;

                                    ////////customerParty.contact.Name = ClientInfo.Name;
                                    ////////customerParty.contact.Telephone = ClientInfo.Phone;
                                    ////////customerParty.contact.ElectronicMail = ClientInfo.Email;
                                    ////////customerParty.contact.Note = string.Empty;
                                }

                            }
                        }

                        if (InvoiceType == 1 || InvoiceType == 2)
                        {
                            // فى حالة فاتورة مبسطة وفاتورة ملخصة هانكتب تاريخ التسليم واخر تاريخ التسليم
                            _invoice.delivery.ActualDeliveryDate = InvoiceInfo.DeliveryTime.Value.ToString("yyyy-MM-dd"); // 40
                            _invoice.delivery.LatestDeliveryDate = InvoiceInfo.DeliveryTime.Value.ToString("yyyy-MM-dd"); // 41
                        }

                        if (InvoiceType == 3 || InvoiceType == 4)
                        {
                            foreach (var PaymentList in InvoicePaymentList)
                            {

                                PaymentMeans paymentMeans = new PaymentMeans();

                                if (PaymentList.PaymentType == 1)
                                {
                                    ///paymentMeans.PaymentMeansCode = "10"; /// 42 // 30
                                    paymentMeans.InstructionNote = PaymentList.Details; // 43 // Cancellation or suspension of the supplies after its occurrence either wholly or partially
                                    _invoice.paymentmeans.Add(paymentMeans);
                                }
                                else if (PaymentCreditList.Contains(PaymentList.PaymentType))
                                {
                                    ///paymentMeans.PaymentMeansCode = "30";  /// 42 // 30
                                    paymentMeans.InstructionNote = PaymentList.Details;  // 43 // Cancellation or suspension of the supplies after its occurrence either wholly or partially

                                    //The payment terms, payment is credit (Need to verify the ID of the PayeeFinancialAccount)
                                    ///paymentMeans.payeefinancialaccount = new PayeeFinancialAccount 
                                    ///{
                                    ///    paymentnote = PaymentList.Details, /// 44 // Payment by credit
                                    ///    ID = PaymentList.PaymentType.ToString(), /// 45 // SA000000000000001
                                    ///};

                                    _invoice.paymentmeans.Add(paymentMeans);
                                }
                                else if (PaymentTransferList.Contains(PaymentList.PaymentType))
                                {
                                    ///paymentMeans.PaymentMeansCode = "42";  /// 42 // 30
                                    paymentMeans.InstructionNote = PaymentList.Details;  // 43 // Cancellation or suspension of the supplies after its occurrence either wholly or partially
                                    _invoice.paymentmeans.Add(paymentMeans);
                                }
                                else if (PaymentOtherList.Contains(PaymentList.PaymentType))
                                {
                                    ///paymentMeans.PaymentMeansCode = "1";  /// 42 // 30
                                    paymentMeans.InstructionNote = PaymentList.Details;  // 43 // Cancellation or suspension of the supplies after its occurrence either wholly or partially
                                    _invoice.paymentmeans.Add(paymentMeans);
                                }
                            }
                        }

                        foreach (var ProductList in InvoiceProductList)
                        {
                            var ProductName = db.ScPackages.Where(x => x.AutoID == ProductList.PackageID).Select(x => x.PkgName).First();

                            InvoiceLine invline = new InvoiceLine();
                            invline.ID = Guid.NewGuid().ToString(); // 97 // Mandatory Invoice line identifier
                            invline.InvoiceQuantity = (decimal)ProductList.Qnt; // 103 // Mandatory Invoiced quantity
                            invline.LineExtensionAmount = ProductList.TotalPrice.GetValueOrDefault(0); // 105 // Mandatory Invoice line net amount
                            invline.taxTotal.TaxAmount = ProductList.TaxAmount.GetValueOrDefault(0);//Mandatory Invoice line identifier
                            invline.taxTotal.RoundingAmount = ProductList.TotalPrice.GetValueOrDefault(0) - ProductList.TaxAmount.GetValueOrDefault(0);//Mandatory Line amount inclusive VAT

                            invline.item.Name = ProductName;//Mandatory Item name

                            invline.price.PriceAmount = (decimal)ProductList.TotalPrice; //Mandatory Currency for item net price

                            invline.item.BuyersItemIdentificationID = string.Empty;//Item Buyer's identifier
                            invline.item.SellersItemIdentificationID = string.Empty;//Item Seller's identifier
                            invline.item.StandardItemIdentificationID = string.Empty;//Item standard identifier

                            if (ProductList.TaxRate == 0)
                            {
                                invline.item.classifiedTaxCategory.ID = "Z"; // Mandatory كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.ID = "Z"; // كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReasonCode = "VATEX-SA-HEA"; // كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReason = "Private healthcare to citizen"; // كود الضريبة
                                invline.item.classifiedTaxCategory.taxScheme.ID = "Z"; //Mandatory Tax scheme ID
                            }
                            else
                            {
                                invline.item.classifiedTaxCategory.ID = "S"; // كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.ID = "S"; // كود الضريبة Mandatory
                                invline.item.classifiedTaxCategory.Percent = (decimal)ProductList.TaxRate; // نسبة الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.Percent = (decimal)ProductList.TaxRate; // نسبة الضريبة
                                invline.taxTotal.TaxSubtotal.TaxAmount = ProductList.TaxAmount.GetValueOrDefault(0);
                                invline.item.classifiedTaxCategory.taxScheme.ID = "S"; //Mandatory Tax scheme ID
                            }

                            invline.price.EncludingVat = false;
                            invline.price.BaseQuantity = ProductList.Qnt.GetValueOrDefault(0);//	Item price base quantity


                            if (ProductList.Discount > 0)
                            {
                                AllowanceCharge allowance = new AllowanceCharge();
                                allowance.ChargeIndicator = false; // 46 // Use “true” when informing about Charges and “false” when informing about Allowances.
                                allowance.MultiplierFactorNumeric = InvoiceInfo.SsDiscount.GetValueOrDefault(0); // 47 // 0 // The percentage that may be used, in conjunction with the document level allowance base amount, to calculate the document level allowance or charge amount. To state 20%, use value 20.
                                allowance.Amount = (decimal)ProductList.Discount; // 48 // The amount of an allowance or a charge, without VAT. Must be rounded to maximum 2 decimals
                                allowance.BaseAmount = InvoiceInfo.TotalPrice.GetValueOrDefault(0) * InvoiceInfo.SsDiscount.GetValueOrDefault(0) / 100; // 50 // 0 // the amount we will apply percentage on example (MultiplierFactorNumeric=10 ,BaseAmount=1000 then AllowanceAmount will be 100 SAR) // in case we will make discount as Amount //allowance.BaseAmount = (decimal)ProductList.Discount; // The base amount that may be used, in conjunction with the document level allowance or charge percentage, to calculate the document level allowance or charge amount. Must be rounded to maximum 2 decimals
                                allowance.taxCategory.ID = "S"; // 52 /// S means standard rated ,Z means Zero rated ,E means Exempt from vat ,O means Not Subject to VAT
                                allowance.taxCategory.Percent = (decimal)ProductList.TaxRate; // 53 // نسبة الضريبة
                                /// allowance.AllowanceChargeReason = "discount"; /// 54 // Document level allowance or charge reason
                                /// allowance.AllowanceChargeReasonCode = ""; /// 55 // Document level allowance (subset of codelist (UNCL5189) or charge reason code (UNCL7161)
                                allowance.taxCategory.taxScheme.ID = "VAT";
                                allowance.taxCategory.TaxExemptionReasonCode = string.Empty;
                                allowance.taxCategory.TaxExemptionReason = string.Empty;
                                //////_invoice.allowanceCharges.Add(allowance);

                                invline.allowanceCharges.Add(allowance);

                                invline.price.allowanceCharge.BaseAmount = allowance.BaseAmount;
                            }

                            _invoice.InvoiceLines.Add(invline);

                            invoicetotal += (decimal)ProductList.TotalPrice;
                        }

                        _invoice.legalMonetaryTotal.LineExtensionAmount = InvoiceInfo.SsPackagePrice.GetValueOrDefault(0) - InvoiceInfo.SsPackageTax.GetValueOrDefault(0); // 68 // Mandatory  Sum of all Invoice line net amounts in the Invoice  without VAT // Sum of all Invoice line net amounts in the Invoice. Must be rounded to maximum 2 decimals.
                        _invoice.legalMonetaryTotal.AllowanceTotalAmount = InvoiceInfo.SsDiscount.GetValueOrDefault(); // 70 // Sum of all allowances on document level in the Invoice. Must be rounded to maximum 2 decimals.
                        _invoice.legalMonetaryTotal.ChargeTotalAmount = 0; // 72 // Sum of all charges on document level in the Invoice. Must be rounded to maximum 2 decimals.
                        _invoice.legalMonetaryTotal.TaxExclusiveAmount = InvoiceInfo.SsPackagePrice.GetValueOrDefault(0) - InvoiceInfo.SsPackageTax.GetValueOrDefault(0); // 74 // The total amount of the Invoice with VAT. Must be rounded to maximum 2 decimals.
                        _invoice.legalMonetaryTotal.TaxInclusiveAmount = InvoiceInfo.TotalPrice.GetValueOrDefault(0); // 80 // The total amount of the Invoice with VAT. Must be rounded to maximum 2 decimals.
                        _invoice.legalMonetaryTotal.PrepaidAmount = 0; // 82 // The sum of amounts which have been paid in advance. Must be rounded to maximum 2 decimals.
                        _invoice.legalMonetaryTotal.PayableRoundingAmount = 0; // 84 // Amount which must be added to total in order to round off the payment amount.
                        _invoice.legalMonetaryTotal.PayableAmount = (decimal)invoicetotal; // 86 //The outstanding amount that is requested to be paid. Must be rounded to maximum 2 decimals.

                        _invoice.TaxTotal.TaxAmount = InvoiceInfo.SsPackageTax.GetValueOrDefault(0); // 76 // Mandatory Invoice total VAT amount
                        _invoice.TaxCurrencyCode = "SAR"; // Mandatory Currency for invoice total VAT amount in accounting currency
                        _invoice.TaxTotal.TaxSubtotal.TaxableAmount = (decimal)invoicetotal; // 88 // Mandatory VAT category taxable amount
                        _invoice.TaxTotal.TaxSubtotal.TaxAmount = (decimal)invoicetotal; // 90 // Mandatory VAT category tax amount
                        _invoice.TaxTotal.TaxSubtotal.taxCategory.ID = "S"; // 92 // Mandatory  VAT category code
                        _invoice.TaxTotal.TaxSubtotal.taxCategory.Percent = (decimal)InvoiceProductList.FirstOrDefault()?.TaxRate.GetValueOrDefault(0); // 93
                        _invoice.TaxTotal.TaxSubtotal.taxCategory.TaxExemptionReason = string.Empty; // 94
                        _invoice.TaxTotal.TaxSubtotal.taxCategory.taxScheme.ID = "VAT"; // 96 // Mandatory Tax scheme ID

                        //InvoiceLine preitem = new InvoiceLine();

                        //preitem.price.PriceAmount = 0;
                        //preitem.InvoiceQuantity = 0;
                        //preitem.item.Name = "prepaid item";
                        //var doc = new DocumentReference();
                        //doc.ID = "";
                        //doc.IssueDate = "";
                        //doc.IssueTime = "";
                        //doc.UUID = "";
                        //doc.DocumentTypeCode = 386;
                        //preitem.documentReferences.Add(doc); 

                        return _invoice;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [NonAction]
        public Invoice ZatcaSalesReturn(int InvoiceID)
        {
            try
            {
                using (var db = new ShoppingCartConnection())
                {
                    var _invoice = new Invoice();
                    var InvoiceInfo = db.ScSalesReturns.Where(x => x.AutoID == InvoiceID).SingleOrDefault();
                    var InvoiceOriginalInfo = db.ScSalesInvoices.Where(x => x.AutoID == InvoiceInfo.SalesRequestNo).SingleOrDefault();
                    if (InvoiceInfo != null)
                    {
                        var InvInfo = db.ScInventories.Where(x => x.AutoID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).SingleOrDefault();
                        var DeviceInfo = db.CgMngDevices.Where(x => x.InventoryID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).First();
                        var CSIDInfo = db.CgMngDevicesZatcas.Where(x => x.DeviceID == DeviceInfo.AutoID && x.IsActive == true && x.IsTrash == false).First();

                        var WebsiteInfo = db.MyPortalWebsites.Where(x => x.AutoID == InvInfo.WebsiteID).SingleOrDefault();
                        var WebsiteIdentifyCRN = db.MyPortalWebsiteIdentifications.Where(x => x.WebsiteId == InvInfo.WebsiteID && x.TypeId == 1).FirstOrDefault();
                        var WebsiteIdentifyTAX = db.MyPortalWebsiteIdentifications.Where(x => x.WebsiteId == InvInfo.WebsiteID && x.TypeId == 2).FirstOrDefault();
                        var SupplierPartyInfo = new Invoice();

                        if (WebsiteIdentifyCRN != null && WebsiteIdentifyTAX != null)
                        {
                            // بيانات البائع 
                            SupplierPartyInfo.SupplierParty.partyLegalEntity.RegistrationName = WebsiteInfo.Title; // "شركة الصناعات الغذائية المتحده"; // اسم الشركة المسجل قى الهيئة
                            SupplierPartyInfo.SupplierParty.postalAddress.StreetName = WebsiteInfo.StreetName; // "شارع تجربة"; // اجبارى
                            SupplierPartyInfo.SupplierParty.postalAddress.AdditionalStreetName = WebsiteInfo.AdditionalStreetName; // "شارع اضافى"; // اختيارى
                            SupplierPartyInfo.SupplierParty.postalAddress.BuildingNumber = WebsiteInfo.BuildingNumber; // "1234"; // اجبارى رقم المبنى
                            SupplierPartyInfo.SupplierParty.postalAddress.PlotIdentification = WebsiteInfo.PlotIdentification.ToString(); // "9833";
                            SupplierPartyInfo.SupplierParty.postalAddress.CityName = WebsiteInfo.City; // "taif";
                            SupplierPartyInfo.SupplierParty.postalAddress.PostalZone = WebsiteInfo.PostalZone; // "12345"; // الرقم البريدي
                            SupplierPartyInfo.SupplierParty.postalAddress.CountrySubentity = WebsiteInfo.CountrySubentity; // "المحافظة"; // اسم المحافظة او المدينة مثال (مكة) اختيارى
                            SupplierPartyInfo.SupplierParty.postalAddress.CitySubdivisionName = WebsiteInfo.CitySubdivisionName; // "اسم المنطقة"; // اسم المنطقة او الحى 

                            // سجلات البائع 
                            SupplierPartyInfo.SupplierParty.partyIdentification.ID = WebsiteIdentifyCRN.Id; //"2050012095"; //هنا رقم السجل التجارى للشركة
                            SupplierPartyInfo.SupplierParty.partyIdentification.schemeID = WebsiteIdentifyCRN.SchemeId; // "CRN";
                            SupplierPartyInfo.SupplierParty.postalAddress.country.IdentificationCode = WebsiteIdentifyCRN.Code;// "SA";
                            SupplierPartyInfo.SupplierParty.partyTaxScheme.CompanyID = WebsiteIdentifyTAX.Id;// "300518376300003";  // رقم التسجيل الضريبي
                        }

                        List<int?> PaymentCreditList = new List<int?>() { 4, 3, 2, 6, 7 };
                        List<int?> PaymentOtherList = new List<int?>() { 8, 9, 5 };
                        List<int?> PaymentTransferList = new List<int?>() { 11, 12, 13 };

                        decimal? invoicetotal = 0;


                        _invoice.ID = InvoiceInfo.Name; // مثال SME00010
                        _invoice.IssueDate = InvoiceInfo.InvoicesDate.ToString("yyyy-MM-dd"); // "2023-02-07"
                        _invoice.IssueTime = InvoiceInfo.InvoicesDate.TimeOfDay.ToString("HH:mm:ss"); // "09:32:40"
                        _invoice.SupplierParty = SupplierPartyInfo.SupplierParty;

                        // $$$ invoice type code
                        // 388 فاتورة  
                        // ************************** المردود بيكون 
                        // 383 اشعار مدين
                        // 381 اشعار دائن
                        _invoice.invoiceTypeCode.id = 388;

                        //if (InvoiceInfo.InvoiceType == 0) // نقدي
                        //{
                        //    _invoice.invoiceTypeCode.id = 388;
                        //}
                        //else
                        //{
                        //    _invoice.invoiceTypeCode.id = 383;
                        //}

                        // $$$ invoice type name
                        // inv.invoiceTypeCode.Name based on format NNPNESB
                        // NN 01 فاتورة عادية
                        // NN 02 فاتورة مبسطة
                        // P فى حالة فاتورة لطرف ثالث نكتب 1 فى الحالة الاخرى نكتب 0
                        // N قی حالة فاتورة اسمية نكتب 1 وفى الحالة الاخرى نكتب 0
                        // E فى حالة فاتورة للصادرات نكتب 1 وفى الحالة الاخرى نكتب 0
                        // S قى حالة فاتورة ملخصة نكتب 1 وفى الحالة الاخرى نكتب 0
                        // B قى حالة فاتورة ذاتية نكتب 1
                        // B قى حالة ان الفاتورة صادرات=1 لايمكن ان تكون الفاتورة ذاتية =1
                        // string InvoiceTypeName = "0100000" "0200000";

                        _invoice.invoiceTypeCode.Name = "0200000";

                        ////////////////// Still
                        // هنا ممكن اضيف ال pih من قاعدة البيانات  
                        //Previous Invoice Hash (PIH) Invoice Counter Value (ICV)
                        //_invoice.AdditionalDocumentReferencePIH.EmbeddedDocumentBinaryObject = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";

                        // قيمة عداد الفاتورة
                        _invoice.AdditionalDocumentReferenceICV.UUID = Guid.NewGuid(); // Convert.ToInt32(InvoiceInfo.AutoID); // لابد ان يكون ارقام فقط


                        if (InvoiceInfo.SupplierNumber != null)
                        {
                            var ClientInfo = db.ScClients.Where(x => x.AutoID == InvoiceInfo.SupplierNumber).SingleOrDefault();
                            var CityName = db.ClsCities.Where(x => x.AutoID == ClientInfo.City).Select(x => x.Name).SingleOrDefault();

                            // Declare customer variables
                            string customerIdValue = "";
                            string customerSchemeId = "CRN";
                            string customerStreet = "";
                            string customerBuilding = "";
                            string customerSubdivision = "";
                            string customerCity = "";
                            string customerPostalZone = "";
                            string customerCountryCode = "SA";
                            string customerVatNumber = "";
                            string customerName = "";

                            if (ClientInfo != null)
                            {
                                if (ClientInfo.AccountNo != null)
                                {
                                    // بيانات المشتري
                                    var ClientIdentifyCRN = db.ScClientIdentifications.Where(x => x.ClientId == InvoiceInfo.SupplierNumber && x.TypeId == 1).FirstOrDefault();
                                    var ClientIdentifyTAX = db.ScClientIdentifications.Where(x => x.ClientId == InvoiceInfo.SupplierNumber && x.TypeId == 2).FirstOrDefault();

                                    customerIdValue = ClientIdentifyCRN?.Id ?? "";
                                    customerSchemeId = ClientIdentifyCRN?.SchemeId ?? "CRN";
                                    customerStreet = ClientInfo.StreetName ?? "";
                                    customerBuilding = ClientInfo.BuildingNumber ?? "";
                                    customerSubdivision = ClientInfo.CitySubdivisionName ?? "";
                                    customerCity = CityName ?? "";
                                    customerPostalZone = ClientInfo.PostalZone ?? "";
                                    customerCountryCode = ClientIdentifyCRN?.Code ?? "SA";
                                    customerVatNumber = ClientIdentifyTAX?.Id ?? "";
                                    customerName = ClientInfo.Name ?? "";
                                }
                            }
                        }

                        _invoice.DocumentCurrencyCode = "SAR";
                        _invoice.TaxCurrencyCode = "SAR";
                        //inv.CurrencyRate = 3.75m;

                        ////// ************************** المردود بيكون 
                        ////if (_invoice.invoiceTypeCode.id == 383 || _invoice.invoiceTypeCode.id == 381)
                        ////{
                        ////    // فى حالة ان اشعار دائن او مدين فقط هانكتب رقم الفاتورة اللى اصدرنا الاشعار ليها
                        ////    InvoiceDocumentReference invoiceDocumentReference = new InvoiceDocumentReference();
                        ////    invoiceDocumentReference.ID = "Invoice Number: 354; Invoice Issue Date: 2021-02-10"; // اجبارى
                        ////    _invoice.billingReference.invoiceDocumentReferences.Add(invoiceDocumentReference);
                        ////}

                        if (_invoice.invoiceTypeCode.Name.Substring(0, 2) == "01")
                        {
                            // فى حالة فاتورة مبسطة وفاتورة ملخصة هانكتب تاريخ التسليم واخر تاريخ التسليم
                            _invoice.delivery.ActualDeliveryDate = InvoiceOriginalInfo.DeliveryTime.Value.ToString("yyyy-MM-dd");
                            _invoice.delivery.LatestDeliveryDate = InvoiceOriginalInfo.DeliveryTime.Value.ToString("yyyy-MM-dd");
                        }

                        // بيانات المدفوعات
                        //Payment means code (BT-81) in an invoice must contain one of the values: 10 - In cash, 30 - Credit, 42 - Payment to bank account, 48 - Bank card, 1 - Instrument not defined (Free text). In the simplified tax invoice and associated credit notes and debit notes (KSA-2, position 1 and 2 = 02) this value is optional
                        var InvoicePaymentList = db.ScSalesInvoicePaymentsTypes.Where(x => x.InvoiceID == InvoiceInfo.AutoID).ToList();

                        foreach (var PaymentList in InvoicePaymentList)
                        {
                            // 1   كاش Cash    1   1   nothing
                            // 4   مدى Mada    1   4   nothing
                            // 3   ماستركارد MasterCard    1   3   nothing
                            // 2   فيزا Visa   1   2   nothing
                            // 6   قطاف Qitaf  1   6   nothing
                            // 7   Stc Pay    1   7   nothing
                            // 8   نقاط Point  1   8   nothing
                            // 9   كوبون Coupon    1   9   nothing
                            // 5   المتبقي 1   5   nothing
                            //تحويل	11
                            //Online Payment  12
                            //شيك 13

                            //10 - In cash
                            //30 - Credit
                            //42 - Payment to bank account
                            //48 - Bank card
                            //1 - Instrument not defined (Free text).
                            //In the simplified tax invoice and associated credit notes and debit notes (KSA-2, position 1 and 2 = 02) this value is optional

                            PaymentMeans paymentMeans = new PaymentMeans();
                            if (PaymentList.PaymentType == 1)
                            {
                                paymentMeans.PaymentMeansCode = "10";
                                paymentMeans.InstructionNote = PaymentList.Details; // "Payment Notes";
                                _invoice.paymentmeans.Add(paymentMeans);
                            }
                            else if (PaymentCreditList.Contains(PaymentList.PaymentType))
                            {
                                paymentMeans.PaymentMeansCode = "30";
                                paymentMeans.InstructionNote = PaymentList.Details; // "Payment Notes";
                                _invoice.paymentmeans.Add(paymentMeans);
                            }
                            else if (PaymentTransferList.Contains(PaymentList.PaymentType))
                            {
                                paymentMeans.PaymentMeansCode = "42";
                                paymentMeans.InstructionNote = PaymentList.Details; // "Payment Notes";
                                _invoice.paymentmeans.Add(paymentMeans);
                            }
                            else if (PaymentOtherList.Contains(PaymentList.PaymentType))
                            {
                                paymentMeans.PaymentMeansCode = "1";
                                paymentMeans.InstructionNote = PaymentList.Details; // "Payment Notes";
                                _invoice.paymentmeans.Add(paymentMeans);
                            }
                            else
                            {
                            }
                        }

                        // بيانات المنتجات
                        var InvoiceProductList = db.ScSubSalesReturns.Where(x => x.InvoiceID == InvoiceInfo.AutoID).ToList();

                        foreach (var ProductList in InvoiceProductList)
                        {
                            var ProductName = db.ScPackages.Where(x => x.AutoID == ProductList.PackageID).Select(x => x.PkgName).First();

                            InvoiceLine invline = new InvoiceLine();
                            invline.InvoiceQuantity = (decimal)ProductList.Qnt.GetValueOrDefault();
                            invline.item.Name = ProductName;

                            if (ProductList.TaxRate == 0)
                            {
                                invline.item.classifiedTaxCategory.ID = "Z"; // كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.ID = "Z"; // كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReasonCode = "VATEX-SA-HEA"; // كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReason = "Private healthcare to citizen"; // كود الضريبة
                            }
                            else
                            {
                                invline.item.classifiedTaxCategory.ID = "S"; // كود الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.ID = "S"; // كود الضريبة
                                invline.item.classifiedTaxCategory.Percent = (decimal)ProductList.TaxRate; // نسبة الضريبة
                                invline.taxTotal.TaxSubtotal.taxCategory.Percent = (decimal)ProductList.TaxRate; // نسبة الضريبة
                            }
                            invline.price.EncludingVat = false;
                            invline.price.PriceAmount = (decimal)ProductList.TotalPrice;

                            _invoice.InvoiceLines.Add(invline);


                            // ممكن نطلعها برا loop لو كان الخصم عام
                            if (ProductList.Discount > 0)
                            {
                                AllowanceCharge allowance = new AllowanceCharge();
                                // فى حالة الرسوم
                                // allowanceCharge.ChargeIndicator = true;
                                // فى حالة الخصم
                                allowance.ChargeIndicator = false;
                                //write this lines in case you will make discount as percentage
                                allowance.MultiplierFactorNumeric = 0; //dscount percentage like 10
                                allowance.BaseAmount = 0; // the amount we will apply percentage on example (MultiplierFactorNumeric=10 ,BaseAmount=1000 then AllowanceAmount will be 100 SAR)

                                // in case we will make discount as Amount 
                                allowance.Amount = (decimal)ProductList.Discount; // 
                                allowance.AllowanceChargeReasonCode = ""; // allowanceCharge.AllowanceChargeReasonCode = "90"; // سبب الخصم على مستوى المنتج
                                allowance.AllowanceChargeReason = "discount"; //سبب الخصم
                                /// S means standard rated
                                /// Z means Zero rated
                                /// E means Exempt from vat
                                /// O means Not Subject to VAT
                                allowance.taxCategory.ID = "S";// كود الضريبة
                                allowance.taxCategory.Percent = (decimal)ProductList.TaxRate;// نسبة الضريبة
                                                                                             //فى حالة عندى اكثر من خصم بعمل loop على الاسطر السابقة


                                _invoice.allowanceCharges.Add(allowance);
                            }

                            invoicetotal += (decimal)ProductList.TotalPrice.GetValueOrDefault(0);
                        }

                        // في حال كان فيه دفعة مسبقة
                        _invoice.legalMonetaryTotal.PrepaidAmount = 0;
                        _invoice.legalMonetaryTotal.PayableAmount = (decimal)invoicetotal;



                        //InvoiceLine preitem = new InvoiceLine();

                        //preitem.price.PriceAmount = 0;
                        //preitem.InvoiceQuantity = 0;
                        //preitem.item.Name = "prepaid item";
                        //var doc = new DocumentReference();
                        //doc.ID = "";
                        //doc.IssueDate = "";
                        //doc.IssueTime = "";
                        //doc.UUID = "";
                        //doc.DocumentTypeCode = 386;
                        //preitem.documentReferences.Add(doc); 

                        _invoice.cSIDInfo.CertPem = CSIDInfo.CSR;
                        _invoice.cSIDInfo.PrivateKey = CSIDInfo.PrivateKey;

                        return _invoice;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [NonAction]
        public static string GetZatcaApiLink(Mode mode)
        {
            try
            {
                string link;
                string linktype;

                link = "https://gw-fatoora.zatca.gov.sa/e-invoicing/"; //TpLibrary.GetServerLinks();

                if (mode == Mode.Production)
                    linktype = "core/";
                else if (mode == Mode.Simulation)
                    linktype = "simulation/";
                else
                    linktype = "developer-portal/";

                link += linktype;

                return link;
            }
            catch (Exception ex)
            {
                return "https://gw-fatoora.zatca.gov.sa/e-invoicing/simulation/";
            }
        }

        [NonAction]
        private AuthenticationResult Authentication(string Auth_key)
        {
            using (var db = new ShoppingCartConnection())
            {
                var result = new AuthenticationResult();
                var obj = db.EmployeePosAuthentications.Where(x => x.Auth_key == Auth_key).SingleOrDefault();
                result.UserLoginInfo = obj;
                result.IsAuthenticated = (obj != null) ? true : false;
                return result;
            }
        }

        [NonAction]
        private static long? GetWebID(int empId)
        {
            using (var db = new ShoppingCartConnection())
            {
                var _IsTrue = db.Employees
                      .Where(x => x.EmployeeID == empId && x.IsTrash == false && x.IsActive == true)
                      .Select(x => x.WebsiteID)
                      .SingleOrDefault();

                return _IsTrue != null ? _IsTrue : 0;
            }
        }

        internal class AuthenticationResult
        {
            public EmployeePosAuthentication UserLoginInfo { get; set; }
            public bool IsAuthenticated { get; set; }
        }
    }
}
