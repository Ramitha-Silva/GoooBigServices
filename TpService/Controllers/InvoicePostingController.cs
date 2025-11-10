using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using TpService.Lib;
using TpService.Models;
using TpService.Models.Data;
using ZatcaIntegrationSDK;
using ZatcaIntegrationSDK.BLL;
using ZatcaIntegrationSDK.HelperContracts;
using System.Xml;

namespace TpService.Controllers
{
    /// <summary>
    /// ZATCA API modes
    /// </summary>
    public enum Mode
    {
        developer,
        Simulation,
        Production
    }

    [AllowCrossSite]
    [RoutePrefix("InvoicePosting")]
    public class InvoicePostingController : Controller
    {
        /// <summary>
        /// Posts invoice to ZATCA using direct XML generation from database
        /// </summary>
        /// <param name="invoiceid">Invoice ID to post</param>
        /// <param name="invoiceType">Invoice type (1=standard, 2=simplified)</param>
        /// <returns>JSON response with status and result</returns>
        [HttpPost]
        [Route("PostInvoice")]
        public ActionResult PostInvoice(int invoiceid, int invoiceType = 2)
        {
            ComplianceCsrResponse _result = new ComplianceCsrResponse();
            try
            {
                using (var db = new ShoppingCartConnection())
                {
                    var InvoiceInfo = db.ScSalesInvoices.Where(x => x.AutoID == invoiceid).SingleOrDefault();

                    if (InvoiceInfo == null)
                    {
                        return Json(new { StatusCode = 404, msg = "Invoice not found" }, JsonRequestBehavior.AllowGet);
                    }

                    var InvInfo = db.ScInventories.Where(x => x.AutoID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();

                    var CSIDInfo = db.CgMngDevicesZatcas.Where(x => x.IsActive == true && x.IsTrash == false).FirstOrDefault();

                    if (CSIDInfo == null)
                    {
                        return Json(new { StatusCode = 202, msg = "CSID Info not found" }, JsonRequestBehavior.AllowGet);
                    }

                    // Generate XML using GenerateXml method instead of ZatcaSalesInvoice
                    XmlDocument invoiceXml = GenerateXml(invoiceid, invoiceType); // 1 standard 2 simplified

                    if (invoiceXml == null)
                    {
                        return Json(new { StatusCode = 500, msg = "Failed to generate invoice XML" }, JsonRequestBehavior.AllowGet);
                    }

                    // Save XML to file for processing - use App_Data folder which is writable
                    string appDataPath = Server.MapPath("~/App_Data/");

                    // Create App_Data directory if it doesn't exist
                    if (!Directory.Exists(appDataPath))
                    {
                        Directory.CreateDirectory(appDataPath);
                    }

                    string xmlFilePath = Path.Combine(appDataPath, $"Invoice_{invoiceid}_{DateTime.Now:yyyyMMddHHmmss}.xml");
                    invoiceXml.Save(xmlFilePath);

                    // Process the XML with ZATCA SDK
                    UBLXML ubl = new UBLXML();
                    
                    // Create Invoice object from the database for SDK processing
                    Invoice invoiceObj = new Invoice();
                    invoiceObj.cSIDInfo.CertPem = CSIDInfo.CSR;
                    invoiceObj.cSIDInfo.PrivateKey = CSIDInfo.PrivateKey;
                    
                    // Process the invoice XML with ZATCA SDK to get signed XML, hash, and QR code
                    bool saveXmlFile = true;
                    ZatcaIntegrationSDK.Result res = ubl.GenerateInvoiceXML(invoiceObj, appDataPath, saveXmlFile);

                    // Prepare request body for ZATCA API
                    InvoiceReportingRequest invrequestbody = new InvoiceReportingRequest();
                    invrequestbody.invoice = res.EncodedInvoice;
                    invrequestbody.invoiceHash = res.InvoiceHash;
                    invrequestbody.uuid = res.UUID;

                    string mainUri = GetZatcaApiLink(Mode.developer);

                    HttpClient cons = new HttpClient();
                    cons.BaseAddress = new Uri(mainUri);
                    cons.DefaultRequestHeaders.Accept.Clear();
                    cons.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{CSIDInfo.binarySecurityToken}:{CSIDInfo.secret}")));
                    cons.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    cons.DefaultRequestHeaders.Add("Accept-Version", "V2");
                    cons.DefaultRequestHeaders.Add("accept-language", "en");

                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | SecurityProtocolType.Tls;

                    var content = new StringContent(JsonConvert.SerializeObject(invrequestbody), Encoding.UTF8, "application/json");

                    string partUri = "compliance/invoices";
                    HttpResponseMessage responsePost = cons.PostAsync(partUri, content).Result;
                    var reponsestr = responsePost.Content.ReadAsStringAsync().Result;
                    _result.StatusCode = (int)responsePost.StatusCode;

                    if (_result.StatusCode == 200)
                    {
                        _result = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);
                        return Json(new { StatusCode = 200, result = _result, xmlPath = xmlFilePath, qrCode = res.QRCode }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // Parse error response
                        try
                        {
                            _result = JsonConvert.DeserializeObject<ComplianceCsrResponse>(reponsestr);
                        }
                        catch
                        {
                            _result.ErrorMessage = reponsestr;
                        }
                        return Json(new { StatusCode = _result.StatusCode, result = _result, xmlPath = xmlFilePath, response = reponsestr }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    StatusCode = 400,
                    msg = ex.Message,
                    stackTrace = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Generates UBL 2.1 XML invoice directly from database
        /// </summary>
        /// <param name="invoiceid">Invoice ID</param>
        /// <param name="InvoiceType">Invoice type (1=standard, 2=simplified, 3=debit, 4=credit)</param>
        /// <returns>XmlDocument containing the UBL invoice</returns>
        [NonAction]
        public XmlDocument GenerateXml(int invoiceid, int InvoiceType)
        {
            try
            {
                using (var db = new ShoppingCartConnection())
                {
                    // Get invoice data from database (similar to ZatcaSalesInvoice)
                    var InvoiceInfo = db.ScSalesInvoices.Where(x => x.AutoID == invoiceid).SingleOrDefault();
                    if (InvoiceInfo == null)
                    {
                        return null;
                    }

                    // Get related data from database
                    var InvoiceProductList = db.ScSubSalesInvoices.Where(x => x.InvoiceID == InvoiceInfo.AutoID).ToList();
                    var InvoicePaymentList = db.ScSalesInvoicePaymentsTypes.Where(x => x.InvoiceID == InvoiceInfo.AutoID).ToList();

                    var InvInfo = db.ScInventories.Where(x => x.AutoID == InvoiceInfo.WarehouseID && x.IsActive == true && x.IsTrash == false).FirstOrDefault();
                    var CSIDInfo = db.CgMngDevicesZatcas.Where(x => x.IsActive == true && x.IsTrash == false).FirstOrDefault();

                    var WebsiteInfo = db.MyPortalWebsites.Where(x => x.AutoID == InvInfo.WebsiteID).SingleOrDefault();
                    var WebsiteIdentify = db.MyPortalWebsiteIdentifications.Where(x => x.WebsiteId == InvInfo.WebsiteID && (x.TypeId == 1 || x.TypeId == 2)).ToList();
                    var WebsiteIdentifyCRN = WebsiteIdentify.FirstOrDefault(x => x.TypeId == 1);
                    var WebsiteIdentifyTAX = WebsiteIdentify.FirstOrDefault(x => x.TypeId == 2);

                    List<int?> PaymentCreditList = new List<int?>() { 4, 3, 2, 6, 7 };
                    List<int?> PaymentOtherList = new List<int?>() { 8, 9, 5 };
                    List<int?> PaymentTransferList = new List<int?>() { 11, 12, 13 };

                    XmlDocument newDoc = new XmlDocument();

                    // Create XML declaration
                    XmlDeclaration xmlDeclaration = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    newDoc.AppendChild(xmlDeclaration);

                    // Define namespaces
                    string ns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                    string nsCac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                    string nsCbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                    string nsExt = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

                    // Create root element with namespaces
                    XmlElement root = newDoc.CreateElement("Invoice", ns);
                    root.SetAttribute("xmlns", ns);
                    root.SetAttribute("xmlns:cac", nsCac);
                    root.SetAttribute("xmlns:cbc", nsCbc);
                    root.SetAttribute("xmlns:ext", nsExt);
                    newDoc.AppendChild(root);

                    // Add basic invoice elements from database
                    AppendElement(newDoc, root, "cbc:ProfileID", nsCbc, "reporting:1.0");
                    AppendElement(newDoc, root, "cbc:ID", nsCbc, InvoiceInfo.Name);
                    AppendElement(newDoc, root, "cbc:UUID", nsCbc, Guid.NewGuid().ToString());
                    AppendElement(newDoc, root, "cbc:IssueDate", nsCbc, InvoiceInfo.InvoicesDate.ToString("yyyy-MM-dd"));
                    AppendElement(newDoc, root, "cbc:IssueTime", nsCbc, InvoiceInfo.InvoicesDate.TimeOfDay.ToString("hh\\:mm\\:ss\\.ffffff"));

                    // Invoice Type Code based on InvoiceType parameter
                    int invoiceTypeCodeId = InvoiceType == 1 || InvoiceType == 2 ? 388 : InvoiceType == 3 ? 383 : InvoiceType == 4 ? 381 : 0;
                    string invoiceTypeCodeName = InvoiceType == 1 ? "0100000" : "0200000";

                    XmlElement invoiceTypeCodeElement = AppendElement(newDoc, root, "cbc:InvoiceTypeCode", nsCbc, invoiceTypeCodeId.ToString());
                    invoiceTypeCodeElement.SetAttribute("name", invoiceTypeCodeName);

                    // Add Note if exists
                    if (!string.IsNullOrEmpty(InvoiceInfo.Details))
                    {
                        XmlElement invoiceNote = AppendElement(newDoc, root, "cbc:Note", nsCbc, InvoiceInfo.Details);
                        invoiceNote.SetAttribute("languageID", "ar");
                    }

                    // Add Currency
                    AppendElement(newDoc, root, "cbc:DocumentCurrencyCode", nsCbc, "SAR");
                    AppendElement(newDoc, root, "cbc:TaxCurrencyCode", nsCbc, "SAR");

                    // Add BillingReference for credit/debit notes
                    if (InvoiceType == 3 || InvoiceType == 4)
                    {
                        XmlElement billingReferenceNode = AppendContainerElement(newDoc, root, "cac:BillingReference", nsCac);
                        XmlElement invoiceDocumentReferenceNode = AppendContainerElement(newDoc, billingReferenceNode, "cac:InvoiceDocumentReference", nsCac);
                        AppendElement(newDoc, invoiceDocumentReferenceNode, "cbc:ID", nsCbc, "Invoice Number: 354; Invoice Issue Date: 2021-02-10");
                    }

                    // Add Additional Document Reference (ICV)
                    XmlElement icvRef = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
                    AppendElement(newDoc, icvRef, "cbc:ID", nsCbc, "ICV");
                    AppendElement(newDoc, icvRef, "cbc:UUID", nsCbc, Guid.NewGuid().ToString());

                    // Add Additional Document Reference (PIH)
                    XmlElement pihRef = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
                    AppendElement(newDoc, pihRef, "cbc:ID", nsCbc, "PIH");
                    XmlElement attachment = AppendContainerElement(newDoc, pihRef, "cac:Attachment", nsCac);

                    string pihValue = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";

                    XmlElement pihBinaryObject = AppendElement(newDoc, attachment, "cbc:EmbeddedDocumentBinaryObject", nsCbc, pihValue);
                    pihBinaryObject.SetAttribute("mimeCode", "text/plain");

                    // Add QR Code
                    XmlElement additionalDocumentReferenceNode = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
                    AppendElement(newDoc, additionalDocumentReferenceNode, "cbc:ID", nsCbc, "QR");
                    XmlElement attachmentNode = AppendContainerElement(newDoc, additionalDocumentReferenceNode, "cac:Attachment", nsCac);

                    string qrValue = "ARNBY21lIFdpZGdldOKAmXMgTFREAg8zMTExMTExMTExMDExMTMDFDIwMjEtMDEtMDVUMDk6MzI6NDBaBAYyNTAuMDAFBDAuMDAGLG5CNTVSbENQajFhRjFkcC9zUkcrVTI2aGc4UDA2TTlCdFRNeHNlb2N5ckU9B2BNRVlDSVFDNFZvZlJxdlZJUng3VUoxTzl4Vjl3SjVFUTdBZk5UV3BnTFdEWEhpSlZZd0loQUw1S040TDR6WUhrMnVwWjlOWmUxYjJwK0pldWtTcEM2OXMyZ284ZXNBaVoIWDBWMBAGByqGSM49AgEGBSuBBAAKA0IABGGDDKDmhWAITDv7LXqLX2cmr6+qddUkpcLCvWs5rC2O29W/hS4ajAK4Qdnahym6MaijX75Cg3j4aao7ouYXJ9EJRjBEAiA6CM08lbTXuWwiKOZVBWQ/sbMU7YpAp30Ydq6QuAhYWwIgUkX27AqFMzEONZs37VrCycUjtEsFHED/qFn4XXC1qpQ=";

                    XmlElement qrBinaryObject = AppendElement(newDoc, attachmentNode, "cbc:EmbeddedDocumentBinaryObject", nsCbc, qrValue);
                    qrBinaryObject.SetAttribute("mimeCode", "text/plain");

                    // ADD SIGNATURE BLOCK
                    XmlElement signatureNode = AppendContainerElement(newDoc, root, "cac:Signature", nsCac);
                    AppendElement(newDoc, signatureNode, "cbc:ID", nsCbc, "urn:oasis:names:specification:ubl:signature:Invoice");
                    AppendElement(newDoc, signatureNode, "cbc:SignatureMethod", nsCbc, "urn:oasis:names:specification:ubl:dsig:enveloped:xades");

                    // Add Supplier Information from database
                    AddSupplierPartyToXml(newDoc, root, nsCac, nsCbc, WebsiteInfo, WebsiteIdentifyCRN, WebsiteIdentifyTAX);

                    // Add Customer Information from database
                    AddCustomerPartyToXml(newDoc, root, nsCac, nsCbc, InvoiceInfo, invoiceTypeCodeName, db);

                    // DELIVERY INFORMATION
                    if (InvoiceType == 1 || InvoiceType == 2)
                    {
                        if (InvoiceInfo.DeliveryTime.HasValue)
                        {
                            XmlElement deliveryNode = AppendContainerElement(newDoc, root, "cac:Delivery", nsCac);
                            AppendElement(newDoc, deliveryNode, "cbc:ActualDeliveryDate", nsCbc, InvoiceInfo.DeliveryTime.Value.ToString("yyyy-MM-dd"));
                            AppendElement(newDoc, deliveryNode, "cbc:LatestDeliveryDate", nsCbc, InvoiceInfo.DeliveryTime.Value.ToString("yyyy-MM-dd"));
                        }
                    }

                    // Payment Means
                    if (InvoiceType == 3 || InvoiceType == 4)
                    {
                        AddPaymentMeansToXml(newDoc, root, nsCac, nsCbc, InvoicePaymentList, PaymentCreditList, PaymentTransferList, PaymentOtherList);
                    }

                    // TAX TOTAL NODES
                    AddTaxTotalToXml(newDoc, root, nsCac, nsCbc, InvoiceInfo, InvoiceProductList);

                    // ADD LEGAL MONETARY TOTAL
                    AddLegalMonetaryTotalToXml(newDoc, root, nsCac, nsCbc, InvoiceInfo);

                    // Add invoice lines from database
                    AddInvoiceLinesToXml(newDoc, root, nsCac, nsCbc, InvoiceProductList, invoiceTypeCodeName, db);

                    return newDoc;
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle appropriately
                return null;
            }
        }

        #region XML Helper Methods

        /// <summary>
        /// Helper method to append element with text
        /// </summary>
        [NonAction]
        private XmlElement AppendElement(XmlDocument doc, XmlElement parent, string elementName, string namespaceUri, string innerText)
        {
            XmlElement element = doc.CreateElement(elementName, namespaceUri);
            element.InnerText = innerText;
            parent.AppendChild(element);
            return element;
        }

        /// <summary>
        /// Helper method to append container element
        /// </summary>
        [NonAction]
        private XmlElement AppendContainerElement(XmlDocument doc, XmlElement parent, string elementName, string namespaceUri)
        {
            XmlElement element = doc.CreateElement(elementName, namespaceUri);
            parent.AppendChild(element);
            return element;
        }

        /// <summary>
        /// Adds supplier party information to XML
        /// </summary>
        [NonAction]
        private void AddSupplierPartyToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc, 
            MyPortalWebsite WebsiteInfo, MyPortalWebsiteIdentification WebsiteIdentifyCRN, MyPortalWebsiteIdentification WebsiteIdentifyTAX)
        {
            XmlElement supplierNode = AppendContainerElement(newDoc, root, "cac:AccountingSupplierParty", nsCac);
            XmlElement supplierParty = AppendContainerElement(newDoc, supplierNode, "cac:Party", nsCac);
            XmlElement partyIdentification = AppendContainerElement(newDoc, supplierParty, "cac:PartyIdentification", nsCac);

            XmlElement supplierId = AppendElement(newDoc, partyIdentification, "cbc:ID", nsCbc, WebsiteIdentifyCRN?.Id ?? "");
            supplierId.SetAttribute("schemeID", WebsiteIdentifyCRN?.SchemeId ?? "CRN");

            XmlElement postalAddress = AppendContainerElement(newDoc, supplierParty, "cac:PostalAddress", nsCac);
            AppendElement(newDoc, postalAddress, "cbc:StreetName", nsCbc, WebsiteInfo.StreetName ?? "");
            AppendElement(newDoc, postalAddress, "cbc:BuildingNumber", nsCbc, WebsiteInfo.BuildingNumber ?? "");

            if (!string.IsNullOrEmpty(WebsiteInfo.PlotIdentification?.ToString()))
                AppendElement(newDoc, postalAddress, "cbc:PlotIdentification", nsCbc, WebsiteInfo.PlotIdentification.ToString());

            AppendElement(newDoc, postalAddress, "cbc:CitySubdivisionName", nsCbc, WebsiteInfo.CitySubdivisionName ?? "");
            AppendElement(newDoc, postalAddress, "cbc:CityName", nsCbc, WebsiteInfo.City ?? "");
            AppendElement(newDoc, postalAddress, "cbc:PostalZone", nsCbc, WebsiteInfo.PostalZone ?? "");

            XmlElement country = AppendContainerElement(newDoc, postalAddress, "cac:Country", nsCac);
            AppendElement(newDoc, country, "cbc:IdentificationCode", nsCbc, WebsiteIdentifyCRN?.Code ?? "SA");

            XmlElement partyTaxScheme = AppendContainerElement(newDoc, supplierParty, "cac:PartyTaxScheme", nsCac);
            AppendElement(newDoc, partyTaxScheme, "cbc:CompanyID", nsCbc, WebsiteIdentifyTAX?.Id ?? "");

            XmlElement taxScheme = AppendContainerElement(newDoc, partyTaxScheme, "cac:TaxScheme", nsCac);
            AppendElement(newDoc, taxScheme, "cbc:ID", nsCbc, "VAT");

            XmlElement partyLegalEntity = AppendContainerElement(newDoc, supplierParty, "cac:PartyLegalEntity", nsCac);
            AppendElement(newDoc, partyLegalEntity, "cbc:RegistrationName", nsCbc, WebsiteInfo.Title ?? "");
        }

        /// <summary>
        /// Adds customer party information to XML
        /// </summary>
        [NonAction]
        private void AddCustomerPartyToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc, 
            ScSalesInvoice InvoiceInfo, string invoiceTypeCodeName, ShoppingCartConnection db)
        {
            XmlElement customerNode = AppendContainerElement(newDoc, root, "cac:AccountingCustomerParty", nsCac);
            XmlElement customerParty = AppendContainerElement(newDoc, customerNode, "cac:Party", nsCac);

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

            // Get customer information if available
            if (InvoiceInfo.SupplierNumber != null)
            {
                var ClientInfo = db.ScClients.Where(x => x.AutoID == InvoiceInfo.SupplierNumber).SingleOrDefault();
                if (ClientInfo != null)
                {
                    var CityName = db.ClsCities.Where(x => x.AutoID == ClientInfo.City).Select(x => x.Name).SingleOrDefault();
                    if (ClientInfo.AccountNo != null)
                    {
                        var ClientIdentity = db.ScClientIdentifications.Where(x => x.ClientId == InvoiceInfo.SupplierNumber && (x.TypeId == 1 || x.TypeId == 2)).ToList();
                        var ClientIdentifyCRN = ClientIdentity.FirstOrDefault(x => x.TypeId == 1);
                        var ClientIdentifyTAX = ClientIdentity.FirstOrDefault(x => x.TypeId == 2);

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

            XmlElement cusPartyIdentification = AppendContainerElement(newDoc, customerParty, "cac:PartyIdentification", nsCac);
            XmlElement customerId = AppendElement(newDoc, cusPartyIdentification, "cbc:ID", nsCbc, customerIdValue);
            customerId.SetAttribute("schemeID", customerSchemeId);

            XmlElement cusPostalAddress = AppendContainerElement(newDoc, customerParty, "cac:PostalAddress", nsCac);
            AppendElement(newDoc, cusPostalAddress, "cbc:StreetName", nsCbc, customerStreet);
            AppendElement(newDoc, cusPostalAddress, "cbc:BuildingNumber", nsCbc, customerBuilding);
            AppendElement(newDoc, cusPostalAddress, "cbc:CitySubdivisionName", nsCbc, customerSubdivision);
            AppendElement(newDoc, cusPostalAddress, "cbc:CityName", nsCbc, customerCity);
            AppendElement(newDoc, cusPostalAddress, "cbc:PostalZone", nsCbc, customerPostalZone);

            XmlElement cusCountry = AppendContainerElement(newDoc, cusPostalAddress, "cac:Country", nsCac);
            AppendElement(newDoc, cusCountry, "cbc:IdentificationCode", nsCbc, customerCountryCode);

            if (invoiceTypeCodeName == "0100000")
            {
                XmlElement cusPartyTaxScheme = AppendContainerElement(newDoc, customerParty, "cac:PartyTaxScheme", nsCac);

                if (!string.IsNullOrEmpty(customerVatNumber))
                {
                    AppendElement(newDoc, cusPartyTaxScheme, "cbc:CompanyID", nsCbc, customerVatNumber);
                }

                XmlElement cusTaxScheme = AppendContainerElement(newDoc, cusPartyTaxScheme, "cac:TaxScheme", nsCac);
                AppendElement(newDoc, cusTaxScheme, "cbc:ID", nsCbc, "VAT");
            }

            XmlElement cusPartyLegalEntity = AppendContainerElement(newDoc, customerParty, "cac:PartyLegalEntity", nsCac);
            AppendElement(newDoc, cusPartyLegalEntity, "cbc:RegistrationName", nsCbc, customerName);
        }

        /// <summary>
        /// Adds payment means information to XML
        /// </summary>
        [NonAction]
        private void AddPaymentMeansToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc,
            List<ScSalesInvoicePaymentsType> InvoicePaymentList, List<int?> PaymentCreditList, 
            List<int?> PaymentTransferList, List<int?> PaymentOtherList)
        {
            foreach (var PaymentList in InvoicePaymentList)
            {
                XmlElement paymentMeansNode = AppendContainerElement(newDoc, root, "cac:PaymentMeans", nsCac);

                string paymentMeansCode = "";
                if (PaymentList.PaymentType == 1)
                    paymentMeansCode = "10";
                else if (PaymentCreditList.Contains(PaymentList.PaymentType))
                    paymentMeansCode = "30";
                else if (PaymentTransferList.Contains(PaymentList.PaymentType))
                    paymentMeansCode = "42";
                else if (PaymentOtherList.Contains(PaymentList.PaymentType))
                    paymentMeansCode = "1";

                if (!string.IsNullOrEmpty(paymentMeansCode))
                    AppendElement(newDoc, paymentMeansNode, "cbc:PaymentMeansCode", nsCbc, paymentMeansCode);

                if (!string.IsNullOrEmpty(PaymentList.Details))
                    AppendElement(newDoc, paymentMeansNode, "cbc:InstructionNote", nsCbc, PaymentList.Details);
            }
        }

        /// <summary>
        /// Adds tax total information to XML
        /// </summary>
        [NonAction]
        private void AddTaxTotalToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc,
            ScSalesInvoice InvoiceInfo, List<ScSubSalesInvoice> InvoiceProductList)
        {
            // TAX TOTAL NODE (first instance - summary)
            XmlElement taxTotalNode1 = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac);
            XmlElement taxAmountElement1 = AppendElement(newDoc, taxTotalNode1, "cbc:TaxAmount", nsCbc, InvoiceInfo.SsPackageTax.GetValueOrDefault(0).ToString("F2"));
            taxAmountElement1.SetAttribute("currencyID", "SAR");

            // 2nd tax total node with subtotals
            XmlElement taxTotalNode = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac);
            XmlElement taxAmountElement = AppendElement(newDoc, taxTotalNode, "cbc:TaxAmount", nsCbc, InvoiceInfo.SsPackageTax.GetValueOrDefault(0).ToString("F2"));
            taxAmountElement.SetAttribute("currencyID", "SAR");

            // Tax Subtotal
            decimal totalTaxableAmount = InvoiceInfo.SsPackagePrice.GetValueOrDefault(0) - InvoiceInfo.SsPackageTax.GetValueOrDefault(0);
            decimal firstProductTaxRate = InvoiceProductList.FirstOrDefault()?.TaxRate.GetValueOrDefault(0) ?? 15;

            XmlElement taxSubtotalNode = AppendContainerElement(newDoc, taxTotalNode, "cac:TaxSubtotal", nsCac);
            XmlElement taxableAmountElement = AppendElement(newDoc, taxSubtotalNode, "cbc:TaxableAmount", nsCbc, totalTaxableAmount.ToString("F2"));
            taxableAmountElement.SetAttribute("currencyID", "SAR");

            XmlElement taxSubAmountElement = AppendElement(newDoc, taxSubtotalNode, "cbc:TaxAmount", nsCbc, InvoiceInfo.SsPackageTax.GetValueOrDefault(0).ToString("F2"));
            taxSubAmountElement.SetAttribute("currencyID", "SAR");

            XmlElement taxCategoryInSubtotalNode = AppendContainerElement(newDoc, taxSubtotalNode, "cac:TaxCategory", nsCac);
            XmlElement taxCategoryIdElem = newDoc.CreateElement("cbc", "ID", nsCbc);
            taxCategoryIdElem.InnerText = "S";
            taxCategoryIdElem.SetAttribute("schemeID", "UN/ECE 5305");
            taxCategoryIdElem.SetAttribute("schemeAgencyID", "6");
            taxCategoryInSubtotalNode.AppendChild(taxCategoryIdElem);

            AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:Percent", nsCbc, firstProductTaxRate.ToString("F2"));

            XmlElement taxSchemeInSubtotalNode = AppendContainerElement(newDoc, taxCategoryInSubtotalNode, "cac:TaxScheme", nsCac);
            XmlElement taxSchemeIdElem = AppendElement(newDoc, taxSchemeInSubtotalNode, "cbc:ID", nsCbc, "VAT");
            taxSchemeIdElem.SetAttribute("schemeID", "UN/ECE 5153");
            taxSchemeIdElem.SetAttribute("schemeAgencyID", "6");
        }

        /// <summary>
        /// Adds legal monetary total information to XML
        /// </summary>
        [NonAction]
        private void AddLegalMonetaryTotalToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc, ScSalesInvoice InvoiceInfo)
        {
            XmlElement legalMonetaryTotalNode = AppendContainerElement(newDoc, root, "cac:LegalMonetaryTotal", nsCac);

            XmlElement lineExtElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:LineExtensionAmount", nsCbc, (InvoiceInfo.SsPackagePrice.GetValueOrDefault(0) - InvoiceInfo.SsPackageTax.GetValueOrDefault(0)).ToString("F2"));
            lineExtElement.SetAttribute("currencyID", "SAR");

            XmlElement taxExclusiveElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxExclusiveAmount", nsCbc, (InvoiceInfo.SsPackagePrice.GetValueOrDefault(0) - InvoiceInfo.SsPackageTax.GetValueOrDefault(0)).ToString("F2"));
            taxExclusiveElement.SetAttribute("currencyID", "SAR");

            XmlElement taxInclusiveElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxInclusiveAmount", nsCbc, InvoiceInfo.TotalPrice.GetValueOrDefault(0).ToString("F2"));
            taxInclusiveElement.SetAttribute("currencyID", "SAR");

            if (InvoiceInfo.SsDiscount.GetValueOrDefault(0) > 0)
            {
                XmlElement allowanceTotalElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:AllowanceTotalAmount", nsCbc, InvoiceInfo.SsDiscount.GetValueOrDefault(0).ToString("F2"));
                allowanceTotalElement.SetAttribute("currencyID", "SAR");
            }

            XmlElement payableElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PayableAmount", nsCbc, InvoiceInfo.TotalPrice.GetValueOrDefault(0).ToString("F2"));
            payableElement.SetAttribute("currencyID", "SAR");
        }

        /// <summary>
        /// Adds invoice lines to XML
        /// </summary>
        [NonAction]
        private void AddInvoiceLinesToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc,
            List<ScSubSalesInvoice> InvoiceProductList, string invoiceTypeCodeName, ShoppingCartConnection db)
        {
            foreach (var ProductList in InvoiceProductList)
            {
                var ProductName = db.ScPackages.Where(x => x.AutoID == ProductList.PackageID).Select(x => x.PkgName).First();

                XmlElement invoiceLineNode = AppendContainerElement(newDoc, root, "cac:InvoiceLine", nsCac);
                AppendElement(newDoc, invoiceLineNode, "cbc:ID", nsCbc, Guid.NewGuid().ToString());

                XmlElement invoicedQuantityNode = AppendElement(newDoc, invoiceLineNode, "cbc:InvoicedQuantity", nsCbc, ProductList.Qnt.GetValueOrDefault(0).ToString("F2"));
                invoicedQuantityNode.SetAttribute("unitCode", "PCE");

                XmlElement lineExtensionAmountElement = AppendElement(newDoc, invoiceLineNode, "cbc:LineExtensionAmount", nsCbc, ProductList.TotalPrice.GetValueOrDefault(0).ToString("F2"));
                lineExtensionAmountElement.SetAttribute("currencyID", "SAR");

                // Add line level allowances/charges for standard invoice
                if (invoiceTypeCodeName == "0100000")
                {
                    if (ProductList.Discount > 0)
                    {
                        XmlElement lineAllowanceChargeNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:AllowanceCharge", nsCac);
                        AppendElement(newDoc, lineAllowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "false");
                        AppendElement(newDoc, lineAllowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, "discount");
                        XmlElement lineAmountElement = AppendElement(newDoc, lineAllowanceChargeNode, "cbc:Amount", nsCbc, ProductList.Discount.GetValueOrDefault(0).ToString("F2"));
                        lineAmountElement.SetAttribute("currencyID", "SAR");
                    }
                }

                // Add TaxTotal block for line
                XmlElement lineTaxTotalNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:TaxTotal", nsCac);
                XmlElement lineTaxAmountElement = AppendElement(newDoc, lineTaxTotalNode, "cbc:TaxAmount", nsCbc, ProductList.TaxAmount.GetValueOrDefault(0).ToString("F2"));
                lineTaxAmountElement.SetAttribute("currencyID", "SAR");

                XmlElement lineRoundingAmountElement = AppendElement(newDoc, lineTaxTotalNode, "cbc:RoundingAmount", nsCbc, (ProductList.TotalPrice.GetValueOrDefault(0) - ProductList.TaxAmount.GetValueOrDefault(0)).ToString("F2"));
                lineRoundingAmountElement.SetAttribute("currencyID", "SAR");

                // Add Item block
                XmlElement itemNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:Item", nsCac);
                AppendElement(newDoc, itemNode, "cbc:Name", nsCbc, ProductName);

                XmlElement classifiedTaxCategoryNode = AppendContainerElement(newDoc, itemNode, "cac:ClassifiedTaxCategory", nsCac);
                string taxCategoryId = ProductList.TaxRate == 0 ? "Z" : "S";
                AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:ID", nsCbc, taxCategoryId);
                AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:Percent", nsCbc, ProductList.TaxRate.GetValueOrDefault(0).ToString("F2"));

                if (ProductList.TaxRate == 0)
                {
                    AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:TaxExemptionReasonCode", nsCbc, "VATEX-SA-HEA");
                    AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:TaxExemptionReason", nsCbc, "Private healthcare to citizen");
                }

                XmlElement itemTaxSchemeNode = AppendContainerElement(newDoc, classifiedTaxCategoryNode, "cac:TaxScheme", nsCac);
                AppendElement(newDoc, itemTaxSchemeNode, "cbc:ID", nsCbc, "VAT");

                // Add Price block
                XmlElement priceNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:Price", nsCac);
                XmlElement priceAmountElement = AppendElement(newDoc, priceNode, "cbc:PriceAmount", nsCbc, ProductList.TotalPrice.GetValueOrDefault(0).ToString("F2"));
                priceAmountElement.SetAttribute("currencyID", "SAR");

                XmlElement baseQuantityElement = AppendElement(newDoc, priceNode, "cbc:BaseQuantity", nsCbc, ProductList.Qnt.GetValueOrDefault(0).ToString("F2"));
                baseQuantityElement.SetAttribute("unitCode", "PCE");

                // Add line level allowances/charges in price node for simplified invoice
                if (invoiceTypeCodeName == "0200000")
                {
                    if (ProductList.Discount > 0)
                    {
                        XmlElement priceAllowanceChargeNode = AppendContainerElement(newDoc, priceNode, "cac:AllowanceCharge", nsCac);
                        AppendElement(newDoc, priceAllowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "false");
                        AppendElement(newDoc, priceAllowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, "discount");

                        XmlElement priceLineAmountElement = AppendElement(newDoc, priceAllowanceChargeNode, "cbc:Amount", nsCbc, ProductList.Discount.GetValueOrDefault(0).ToString("F2"));
                        priceLineAmountElement.SetAttribute("currencyID", "SAR");
                    }
                }
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets ZATCA API link based on mode
        /// </summary>
        [NonAction]
        public static string GetZatcaApiLink(Mode mode)
        {
            try
            {
                string link;
                string linktype;

                link = "https://gw-fatoora.zatca.gov.sa/e-invoicing/";

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

        #endregion
    }
}
