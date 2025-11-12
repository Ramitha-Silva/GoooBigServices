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
        public ActionResult PostInvoice(int invoiceid, int invoiceType = 2,int invoiceTypeCodeValue=388)
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

                    InvoiceData invoiceData = LoadInvoiceData(invoiceid, invoiceType, invoiceTypeCodeValue);

          
                    XmlDocument invoiceXml = GenerateXmlInvoice(invoiceData); // 1 standard 2 simplified

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


        public InvoiceData LoadInvoiceData(int invoiceid, int invoiceType,int invoiceTypeCodeVaue)
        {
            using (var db = new ShoppingCartConnection())
            {
                // Load invoice data from database and map to InvoiceData object
                // This is a placeholder implementation; actual mapping logic is needed
                var invoiceData = new InvoiceData();
                // Populate invoiceData properties from database entities
                return invoiceData;
            }
        }

        /// <summary>
        /// Generates UBL 2.1 XML invoice directly from database
        /// </summary>
        /// <param name="invoiceid">Invoice ID</param>
        /// <param name="InvoiceType">Invoice type (1=standard, 2=simplified, 3=debit, 4=credit)</param>
        /// <returns>XmlDocument containing the UBL invoice</returns>
        [NonAction]
        public XmlDocument GenerateXml(int invoiceid, int InvoiceType,int invoiceMode)
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
                    int invoiceTypeCodeId = invoiceMode;
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
                    if (InvoiceType == 383 || InvoiceType == 381)
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

        /// <summary>
        /// Generates UBL 2.1 XML invoice with full UBL compliance including document/line allowances and charges
        /// Converted from VB.NET
        /// </summary>
        /// <param name="invoiceData">Invoice data object containing all required invoice information</param>
        /// <returns>XmlDocument containing the full UBL invoice</returns>
        [NonAction]
        public XmlDocument GenerateXmlInvoice(InvoiceData invoiceData)
        {
            try
            {
                XmlDocument newDoc = new XmlDocument();

                // Create XML declaration
                XmlDeclaration xmlDeclaration = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                newDoc.AppendChild(xmlDeclaration);

                // Define namespaces
                string ns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                string nsCac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                string nsCbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                string nsExt = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

                // Create root element with namespaces in the correct order
                XmlElement root = newDoc.CreateElement("Invoice", ns);
                root.SetAttribute("xmlns", ns);
                root.SetAttribute("xmlns:cac", nsCac);
                root.SetAttribute("xmlns:cbc", nsCbc);
                root.SetAttribute("xmlns:ext", nsExt);
                newDoc.AppendChild(root);

                // Add ProfileID
                AppendElement(newDoc, root, "cbc:ProfileID", nsCbc, "reporting:1.0");

                // Add Invoice ID
                AppendElement(newDoc, root, "cbc:ID", nsCbc, invoiceData.Id);

                // Add UUID
                AppendElement(newDoc, root, "cbc:UUID", nsCbc, invoiceData.InvoiceUUID);

                // Add Issue Date & Time
                AppendElement(newDoc, root, "cbc:IssueDate", nsCbc, invoiceData.IssueDate);
                AppendElement(newDoc, root, "cbc:IssueTime", nsCbc, invoiceData.IssueTime);

                // Add Invoice Type Code
                XmlElement invoiceTypeCodeElement = AppendElement(newDoc, root, "cbc:InvoiceTypeCode", nsCbc, invoiceData.InvoiceTypeCodeValue.ToString());
                invoiceTypeCodeElement.SetAttribute("name", invoiceData.InvoiceTypeCodename);

                // Add Note with language attribute
                if (!string.IsNullOrEmpty(invoiceData.InstructionNote))
                {
                    XmlElement invoiceNote = AppendElement(newDoc, root, "cbc:Note", nsCbc, invoiceData.InstructionNote);
                    invoiceNote.SetAttribute("languageID", "ar");
                }

                // Add Currency codes
                AppendElement(newDoc, root, "cbc:DocumentCurrencyCode", nsCbc, invoiceData.Currency);
                AppendElement(newDoc, root, "cbc:TaxCurrencyCode", nsCbc, invoiceData.TaxCurrency);

                // Add BillingReference for credit/debit notes (381 or 383)
                if (invoiceData.InvoiceTypeCodeValue == 381 || invoiceData.InvoiceTypeCodeValue == 383)
                {
                    XmlElement billingReferenceNode = AppendContainerElement(newDoc, root, "cac:BillingReference", nsCac);
                    XmlElement invoiceDocumentReferenceNode = AppendContainerElement(newDoc, billingReferenceNode, "cac:InvoiceDocumentReference", nsCac);
                    AppendElement(newDoc, invoiceDocumentReferenceNode, "cbc:ID", nsCbc, invoiceData.PreviousInvoiceNumber);
                }

                // Add Additional Document Reference (ICV)
                XmlElement icvRef = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
                AppendElement(newDoc, icvRef, "cbc:ID", nsCbc, "ICV");
                AppendElement(newDoc, icvRef, "cbc:UUID", nsCbc, invoiceData.ICV.ToString());

                // Add Additional Document Reference (PIH)
                XmlElement pihRef = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
                AppendElement(newDoc, pihRef, "cbc:ID", nsCbc, "PIH");
                XmlElement attachment = AppendContainerElement(newDoc, pihRef, "cac:Attachment", nsCac);

                string pihValue = string.IsNullOrEmpty(invoiceData.PIH) 
                    ? "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==" 
                    : invoiceData.PIH;

                XmlElement pihBinaryObject = AppendElement(newDoc, attachment, "cbc:EmbeddedDocumentBinaryObject", nsCbc, pihValue);
                pihBinaryObject.SetAttribute("mimeCode", "text/plain");

                // Add QR Code
                XmlElement additionalDocumentReferenceNode = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
                AppendElement(newDoc, additionalDocumentReferenceNode, "cbc:ID", nsCbc, "QR");
                XmlElement attachmentNode = AppendContainerElement(newDoc, additionalDocumentReferenceNode, "cac:Attachment", nsCac);

                // Default QR value (will be replaced during signing)
                string qrValue = "ARNBY21lIFdpZGdldOKAmXMgTFREAg8zMTExMTExMTExMDExMTMDFDIwMjEtMDEtMDVUMDk6MzI6NDBaBAYyNTAuMDAFBDAuMDAGLG5CNTVSbENQajFhRjFkcC9zUkcrVTI2aGc4UDA2TTlCdFRNeHNlb2N5ckU9B2BNRVlDSVFDNFZvZlJxdlZJUng3VUoxTzl4Vjl3SjVFUTdBZk5UV3BnTFdEWEhpSlZZd0loQUw1S040TDR6WUhrMnVwWjlOWmUxYjJwK0pldWtTcEM2OXMyZ284ZXNBaVoIWDBWMBAGByqGSM49AgEGBSuBBAAKA0IABGGDDKDmhWAITDv7LXqLX2cmr6+qddUkpcLCvWs5rC2O29W/hS4ajAK4Qdnahym6MaijX75Cg3j4aao7ouYXJ9EJRjBEAiA6CM08lbTXuWwiKOZVBWQ/sbMU7YpAp30Ydq6QuAhYWwIgUkX27AqFMzEONZs37VrCycUjtEsFHED/qFn4XXC1qpQ=";

                XmlElement qrBinaryObject = AppendElement(newDoc, attachmentNode, "cbc:EmbeddedDocumentBinaryObject", nsCbc, qrValue);
                qrBinaryObject.SetAttribute("mimeCode", "text/plain");

                // ADD SIGNATURE BLOCK
                XmlElement signatureNode = AppendContainerElement(newDoc, root, "cac:Signature", nsCac);
                AppendElement(newDoc, signatureNode, "cbc:ID", nsCbc, "urn:oasis:names:specification:ubl:signature:Invoice");
                AppendElement(newDoc, signatureNode, "cbc:SignatureMethod", nsCbc, "urn:oasis:names:specification:ubl:dsig:enveloped:xades");

                // Add Supplier Information
                AddSupplierPartyToXmlInvoice(newDoc, root, nsCac, nsCbc, invoiceData.AccountingSupplier);

                // Add Customer Information
                AddCustomerPartyToXmlInvoice(newDoc, root, nsCac, nsCbc, invoiceData.AccountingCustomer, invoiceData.InvoiceTypeCodename);

                // DELIVERY INFORMATION
                if (!string.IsNullOrEmpty(invoiceData.ActualDeliveryDate))
                {
                    XmlElement deliveryNode = AppendContainerElement(newDoc, root, "cac:Delivery", nsCac);
                    AppendElement(newDoc, deliveryNode, "cbc:ActualDeliveryDate", nsCbc, invoiceData.ActualDeliveryDate);
                    AppendElement(newDoc, deliveryNode, "cbc:LatestDeliveryDate", nsCbc, invoiceData.LatestDeliveryDate);
                }

                // Payment Means
                XmlElement paymentMeansNode = AppendContainerElement(newDoc, root, "cac:PaymentMeans", nsCac);
                AppendElement(newDoc, paymentMeansNode, "cbc:PaymentMeansCode", nsCbc, invoiceData.PaymentMeansCode);

                if (invoiceData.InvoiceTypeCodeValue == 381 || invoiceData.InvoiceTypeCodeValue == 383)
                {
                    AppendElement(newDoc, paymentMeansNode, "cbc:InstructionNote", nsCbc, invoiceData.CreditDebitNoteReason);
                }

                // ** Add Document Level Allowances (Repeating Node Based on List)**
                if (invoiceData.DocumentLevelAllowances != null)
                {
                    foreach (var allowance in invoiceData.DocumentLevelAllowances)
                    {
                        XmlElement allowanceChargeNode = AppendContainerElement(newDoc, root, "cac:AllowanceCharge", nsCac);

                        // Add ChargeIndicator element
                        AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "false");

                        // Add AllowanceChargeReason element
                        AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, allowance.AllowanceChargeReason);

                        // Add Amount element and set currencyID attribute
                        XmlElement amountElement = AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, allowance.Amount.ToString("0.00"));
                        amountElement.SetAttribute("currencyID", "SAR");

                        // Add Tax Category under AllowanceCharge
                        XmlElement taxCategoryNode = AppendContainerElement(newDoc, allowanceChargeNode, "cac:TaxCategory", nsCac);

                        // Add ID element inside TaxCategory and set attributes manually
                        XmlElement taxCategoryIdElement1 = newDoc.CreateElement("cbc:ID", nsCbc);
                        taxCategoryIdElement1.InnerText = allowance.TaxCategoryCode;
                        taxCategoryIdElement1.SetAttribute("schemeID", "UN/ECE 5305");
                        taxCategoryIdElement1.SetAttribute("schemeAgencyID", "6");
                        taxCategoryNode.AppendChild(taxCategoryIdElement1);

                        // Add Percent element inside TaxCategory
                        AppendElement(newDoc, taxCategoryNode, "cbc:Percent", nsCbc, allowance.TaxPercent.ToString("0.00"));

                        // Add TaxScheme under TaxCategory
                        XmlElement taxSchemeNode = AppendContainerElement(newDoc, taxCategoryNode, "cac:TaxScheme", nsCac);

                        // Add ID element inside TaxScheme and set attributes manually
                        XmlElement taxSchemeIdElement = newDoc.CreateElement("cbc:ID", nsCbc);
                        taxSchemeIdElement.InnerText = "VAT";
                        taxSchemeIdElement.SetAttribute("schemeID", "UN/ECE 5153");
                        taxSchemeIdElement.SetAttribute("schemeAgencyID", "6");
                        taxSchemeNode.AppendChild(taxSchemeIdElement);
                    }
                }

                // ** Add Document Level Charges (Repeating Node Based on List)**
                if (invoiceData.DocumentLevelCharges != null)
                {
                    foreach (var charges in invoiceData.DocumentLevelCharges)
                    {
                        XmlElement allowanceChargeNode = AppendContainerElement(newDoc, root, "cac:AllowanceCharge", nsCac);

                        // Add ChargeIndicator element
                        AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true");

                        // Add AllowanceChargeReason elements
                        AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReasonCode", nsCbc, "CG");
                        AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, charges.AllowanceChargeReason);

                        // Add Amount element and set currencyID attribute
                        XmlElement amountElement = AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, charges.Amount.ToString("0.00"));
                        amountElement.SetAttribute("currencyID", "SAR");

                        // Add Tax Category under AllowanceCharge
                        XmlElement taxCategoryNode = AppendContainerElement(newDoc, allowanceChargeNode, "cac:TaxCategory", nsCac);

                        XmlElement taxCategoryIdElement2 = newDoc.CreateElement("cbc:ID", nsCbc);
                        taxCategoryIdElement2.InnerText = charges.TaxCategoryCode;
                        taxCategoryIdElement2.SetAttribute("schemeID", "UN/ECE 5305");
                        taxCategoryIdElement2.SetAttribute("schemeAgencyID", "6");
                        taxCategoryNode.AppendChild(taxCategoryIdElement2);

                        // Add Percent element inside TaxCategory
                        AppendElement(newDoc, taxCategoryNode, "cbc:Percent", nsCbc, charges.TaxPercent.ToString("0.00"));

                        // Add TaxScheme under TaxCategory
                        XmlElement taxSchemeNode = AppendContainerElement(newDoc, taxCategoryNode, "cac:TaxScheme", nsCac);

                        // Add ID element inside TaxScheme and set attributes manually
                        XmlElement taxSchemeIdElement = newDoc.CreateElement("cbc:ID", nsCbc);
                        taxSchemeIdElement.InnerText = "VAT";
                        taxSchemeIdElement.SetAttribute("schemeID", "UN/ECE 5153");
                        taxSchemeIdElement.SetAttribute("schemeAgencyID", "6");
                        taxSchemeNode.AppendChild(taxSchemeIdElement);
                    }
                }

                // TAX TOTAL NODE (first instance - summary)
                XmlElement taxTotalNode1 = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac);
                XmlElement taxAmountElement1 = AppendElement(newDoc, taxTotalNode1, "cbc:TaxAmount", nsCbc, invoiceData.TotalTaxAmount.ToString("0.00"));
                taxAmountElement1.SetAttribute("currencyID", "SAR");

                // 2nd tax total node with subtotals
                XmlElement taxTotalNode = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac);
                XmlElement taxAmountElement = AppendElement(newDoc, taxTotalNode, "cbc:TaxAmount", nsCbc, invoiceData.TotalTaxAmount.ToString("0.00"));
                taxAmountElement.SetAttribute("currencyID", "SAR");

                // Add document tax subtotal nodes
                if (invoiceData.DocumentVatCategories != null)
                {
                    foreach (var taxcat in invoiceData.DocumentVatCategories)
                    {
                        XmlElement taxSubtotalNode = AppendContainerElement(newDoc, taxTotalNode, "cac:TaxSubtotal", nsCac);
                        XmlElement taxableAmountElement = AppendElement(newDoc, taxSubtotalNode, "cbc:TaxableAmount", nsCbc, taxcat.CatTaxableAmount.ToString("0.00"));
                        taxableAmountElement.SetAttribute("currencyID", "SAR");

                        // Sub total tax amount is 0 if document taxcode is E or Z
                        string taxAmountValue = (taxcat.VatCategoryCode != "S") ? "0.00" : taxcat.CategoryVatAmount.ToString("0.00");
                        XmlElement taxSubAmountElement = AppendElement(newDoc, taxSubtotalNode, "cbc:TaxAmount", nsCbc, taxAmountValue);
                        taxSubAmountElement.SetAttribute("currencyID", "SAR");

                        XmlElement taxCategoryInSubtotalNode = AppendContainerElement(newDoc, taxSubtotalNode, "cac:TaxCategory", nsCac);

                        XmlElement taxCategoryIdElem = newDoc.CreateElement("cbc:ID", nsCbc);
                        taxCategoryIdElem.InnerText = taxcat.VatCategoryCode;
                        taxCategoryIdElem.SetAttribute("schemeID", "UN/ECE 5305");
                        taxCategoryIdElem.SetAttribute("schemeAgencyID", "6");
                        taxCategoryInSubtotalNode.AppendChild(taxCategoryIdElem);

                        string percentValue = (taxcat.VatCategoryCode != "S") ? "0.00" : taxcat.VatCategoryRate.ToString("0.00");
                        AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:Percent", nsCbc, percentValue);

                        // Add tax exemption for Z or E categories
                        if (taxcat.VatCategoryCode == "Z" || taxcat.VatCategoryCode == "E")
                        {
                            AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:TaxExemptionReasonCode", nsCbc, taxcat.VatExCode);
                            AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:TaxExemptionReason", nsCbc, taxcat.VatExDesc);
                        }

                        XmlElement taxSchemeInSubtotalNode = AppendContainerElement(newDoc, taxCategoryInSubtotalNode, "cac:TaxScheme", nsCac);
                        XmlElement element = AppendElement(newDoc, taxSchemeInSubtotalNode, "cbc:ID", nsCbc, "VAT");
                        element.SetAttribute("schemeID", "UN/ECE 5153");
                        element.SetAttribute("schemeAgencyID", "6");
                    }
                }

                // ADD LEGAL MONETARY TOTAL
                XmlElement legalMonetaryTotalNode = AppendContainerElement(newDoc, root, "cac:LegalMonetaryTotal", nsCac);

                XmlElement lineExtElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:LineExtensionAmount", nsCbc, invoiceData.LineExtensionAmount.ToString("0.00"));
                lineExtElement.SetAttribute("currencyID", "SAR");

                XmlElement taxExclusiveElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxExclusiveAmount", nsCbc, invoiceData.TaxExclusiveAmount.ToString("0.00"));
                taxExclusiveElement.SetAttribute("currencyID", "SAR");

                XmlElement taxInclusiveElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxInclusiveAmount", nsCbc, invoiceData.TaxInclusiveAmount.ToString("0.00"));
                taxInclusiveElement.SetAttribute("currencyID", "SAR");

                if (invoiceData.AllowanceTotalAmount > 0)
                {
                    XmlElement allowanceTotalElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:AllowanceTotalAmount", nsCbc, invoiceData.AllowanceTotalAmount.ToString("0.00"));
                    allowanceTotalElement.SetAttribute("currencyID", "SAR");
                }

                if (invoiceData.ChargeTotalAmount > 0)
                {
                    XmlElement chargeTotalElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:ChargeTotalAmount", nsCbc, invoiceData.ChargeTotalAmount.ToString("0.00"));
                    chargeTotalElement.SetAttribute("currencyID", "SAR");
                }

                if (invoiceData.PrepaidAmount > 0)
                {
                    XmlElement prepaidElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PrepaidAmount", nsCbc, invoiceData.PrepaidAmount.ToString("0.00"));
                    prepaidElement.SetAttribute("currencyID", "SAR");
                }

                if (invoiceData.PayableRoundAmount != 0)
                {
                    XmlElement roundingElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PayableRoundingAmount", nsCbc, invoiceData.PayableRoundAmount.ToString("0.00"));
                    roundingElement.SetAttribute("currencyID", "SAR");
                }

                XmlElement payableElement = AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PayableAmount", nsCbc, invoiceData.PayableAmount.ToString("0.00"));
                payableElement.SetAttribute("currencyID", "SAR");

                // Add invoice lines
                if (invoiceData.InvoiceLines != null)
                {
                    foreach (var invoiceLine in invoiceData.InvoiceLines)
                    {
                        XmlElement invoiceLineNode = AppendContainerElement(newDoc, root, "cac:InvoiceLine", nsCac);
                        AppendElement(newDoc, invoiceLineNode, "cbc:ID", nsCbc, invoiceLine.LineID);

                        XmlElement invoicedQuantityNode = AppendElement(newDoc, invoiceLineNode, "cbc:InvoicedQuantity", nsCbc, invoiceLine.InvoicedQuantity.ToString("0.00"));
                        invoicedQuantityNode.SetAttribute("unitCode", invoiceLine.UnitCode);

                        XmlElement lineExtensionAmountElement = AppendElement(newDoc, invoiceLineNode, "cbc:LineExtensionAmount", nsCbc, invoiceLine.LineExtensionAmount.ToString("0.00"));
                        lineExtensionAmountElement.SetAttribute("currencyID", invoiceLine.LineCurrency);

                        // Add line level allowances/charges for standard invoice
                        if (invoiceData.InvoiceTypeCodename == "0100000")
                        {
                            // Add line level allowances
                            if (invoiceLine.LineLevelAllowances != null)
                            {
                                foreach (var allowance in invoiceLine.LineLevelAllowances)
                                {
                                    XmlElement allowanceChargeNode1 = AppendContainerElement(newDoc, invoiceLineNode, "cac:AllowanceCharge", nsCac);
                                    AppendElement(newDoc, allowanceChargeNode1, "cbc:ChargeIndicator", nsCbc, "false");
                                    AppendElement(newDoc, allowanceChargeNode1, "cbc:AllowanceChargeReason", nsCbc, allowance.Reason);
                                    XmlElement amountEl = AppendElement(newDoc, allowanceChargeNode1, "cbc:Amount", nsCbc, allowance.Amount.ToString("0.00"));
                                    amountEl.SetAttribute("currencyID", invoiceLine.LineCurrency);
                                }
                            }

                            // Add line level charges
                            if (invoiceLine.LineLevelCharges != null)
                            {
                                foreach (var charge in invoiceLine.LineLevelCharges)
                                {
                                    XmlElement allowanceChargeNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:AllowanceCharge", nsCac);
                                    AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true");
                                    AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, charge.Reason);
                                    XmlElement amountEl = AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, charge.Amount.ToString("0.00"));
                                    amountEl.SetAttribute("currencyID", invoiceLine.LineCurrency);
                                }
                            }
                        }

                        // If prepaid line, add prepaid doc reference
                        if (invoiceLine.InvoiceLineType == "PREPAID")
                        {
                            XmlElement prepaidDocumentReferenceNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:DocumentReference", nsCac);
                            AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:ID", nsCbc, invoiceLine.PrepaidInvoiceId);
                            AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:IssueDate", nsCbc, invoiceLine.PrepaidIssueDate);
                            AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:IssueTime", nsCbc, invoiceLine.PrepaidIssueTime);
                            AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:DocumentTypeCode", nsCbc, invoiceLine.PrepaidDocumentCode.ToString());
                        }

                        // Add TaxTotal block
                        XmlElement linetaxTotalNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:TaxTotal", nsCac);
                        XmlElement lineTaxAmtEl = AppendElement(newDoc, linetaxTotalNode, "cbc:TaxAmount", nsCbc, invoiceLine.TaxAmount.ToString("0.00"));
                        lineTaxAmtEl.SetAttribute("currencyID", "SAR");
                        XmlElement lineRoundingAmtEl = AppendElement(newDoc, linetaxTotalNode, "cbc:RoundingAmount", nsCbc, invoiceLine.RoundingAmount.ToString("0.00"));
                        lineRoundingAmtEl.SetAttribute("currencyID", "SAR");

                        // If prepaid line, add tax subtotal
                        if (invoiceLine.InvoiceLineType == "PREPAID")
                        {
                            XmlElement prepaidTaxSubtotalNode = AppendContainerElement(newDoc, linetaxTotalNode, "cac:TaxSubtotal", nsCac);

                            XmlElement taxableAmtEl = AppendElement(newDoc, prepaidTaxSubtotalNode, "cbc:TaxableAmount", nsCbc, invoiceLine.PrepaidTaxSubTotalTaxableAmount.ToString("0.00"));
                            taxableAmtEl.SetAttribute("currencyID", "SAR");
                            XmlElement taxAmtEl = AppendElement(newDoc, prepaidTaxSubtotalNode, "cbc:TaxAmount", nsCbc, invoiceLine.PrepaidTaxSubTotalTaxAmount.ToString("0.00"));
                            taxAmtEl.SetAttribute("currencyID", "SAR");

                            XmlElement prepaidtaxCategoryInSubtotalNode = AppendContainerElement(newDoc, prepaidTaxSubtotalNode, "cac:TaxCategory", nsCac);

                            XmlElement prepaidtaxCategoryIdElement = newDoc.CreateElement("cbc:ID", nsCbc);
                            prepaidtaxCategoryIdElement.InnerText = invoiceLine.TaxCategoryID;
                            prepaidtaxCategoryInSubtotalNode.AppendChild(prepaidtaxCategoryIdElement);

                            AppendElement(newDoc, prepaidtaxCategoryInSubtotalNode, "cbc:Percent", nsCbc, invoiceLine.TaxPercent.ToString("0.00"));

                            XmlElement prepaidtaxSchemeInSubtotalNode = AppendContainerElement(newDoc, prepaidtaxCategoryInSubtotalNode, "cac:TaxScheme", nsCac);
                            AppendElement(newDoc, prepaidtaxSchemeInSubtotalNode, "cbc:ID", nsCbc, "VAT");
                        }

                        // Add Item block
                        XmlElement itemNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:Item", nsCac);
                        AppendElement(newDoc, itemNode, "cbc:Name", nsCbc, invoiceLine.ItemName);

                        XmlElement classifiedTaxCategoryNode = AppendContainerElement(newDoc, itemNode, "cac:ClassifiedTaxCategory", nsCac);
                        AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:ID", nsCbc, invoiceLine.TaxCategoryID);
                        AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:Percent", nsCbc, invoiceLine.TaxPercent.ToString("0.00"));

                        XmlElement itemTaxSchemeNode = AppendContainerElement(newDoc, classifiedTaxCategoryNode, "cac:TaxScheme", nsCac);
                        AppendElement(newDoc, itemTaxSchemeNode, "cbc:ID", nsCbc, "VAT");

                        // Add Price block
                        XmlElement priceNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:Price", nsCac);
                        XmlElement priceAmountElement = AppendElement(newDoc, priceNode, "cbc:PriceAmount", nsCbc, invoiceLine.PriceAmount.ToString("0.00"));
                        priceAmountElement.SetAttribute("currencyID", invoiceLine.LineCurrency);

                        if (invoiceLine.InvoiceLineType != "PREPAID")
                        {
                            XmlElement baseQuantityElement = AppendElement(newDoc, priceNode, "cbc:BaseQuantity", nsCbc, invoiceLine.BaseQuantity.ToString("0.00"));
                            baseQuantityElement.SetAttribute("unitCode", invoiceLine.BaseUnitCode);
                        }

                        // If simplified, then print allowance charges in the price node
                        if (invoiceData.InvoiceTypeCodename == "0200000")
                        {
                            // Add line level allowances
                            if (invoiceLine.LineLevelAllowances != null)
                            {
                                foreach (var allowance in invoiceLine.LineLevelAllowances)
                                {
                                    XmlElement allowanceChargeNode1 = AppendContainerElement(newDoc, priceNode, "cac:AllowanceCharge", nsCac);
                                    AppendElement(newDoc, allowanceChargeNode1, "cbc:ChargeIndicator", nsCbc, "false");
                                    AppendElement(newDoc, allowanceChargeNode1, "cbc:AllowanceChargeReason", nsCbc, allowance.Reason);
                                    XmlElement amountEl = AppendElement(newDoc, allowanceChargeNode1, "cbc:Amount", nsCbc, allowance.Amount.ToString("0.00"));
                                    amountEl.SetAttribute("currencyID", invoiceLine.LineCurrency);
                                }
                            }

                            // Add line level charges
                            if (invoiceLine.LineLevelCharges != null)
                            {
                                foreach (var charge in invoiceLine.LineLevelCharges)
                                {
                                    XmlElement allowanceChargeNode = AppendContainerElement(newDoc, priceNode, "cac:AllowanceCharge", nsCac);
                                    AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true");
                                    AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, charge.Reason);
                                    XmlElement amountEl = AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, charge.Amount.ToString("0.00"));
                                    amountEl.SetAttribute("currencyID", invoiceLine.LineCurrency);
                                }
                            }
                        }
                    }
                }

                return newDoc;
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

        ///// <summary>
        ///// Adds supplier party information to XML
        ///// </summary>
        //[NonAction]
        //private void AddSupplierPartyToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc, 
        //    MyPortalWebsite WebsiteInfo, MyPortalWebsiteIdentification WebsiteIdentifyCRN, MyPortalWebsiteIdentification WebsiteIdentifyTAX)
        //{
        //    XmlElement supplierNode = AppendContainerElement(newDoc, root, "cac:AccountingSupplierParty", nsCac);
        //    XmlElement supplierParty = AppendContainerElement(newDoc, supplierNode, "cac:Party", nsCac);
        //    XmlElement partyIdentification = AppendContainerElement(newDoc, supplierParty, "cac:PartyIdentification", nsCac);

        //    XmlElement supplierId = AppendElement(newDoc, partyIdentification, "cbc:ID", nsCbc, WebsiteIdentifyCRN?.Id ?? "");
        //    supplierId.SetAttribute("schemeID", WebsiteIdentifyCRN?.SchemeId ?? "CRN");

        //    XmlElement postalAddress = AppendContainerElement(newDoc, supplierParty, "cac:PostalAddress", nsCac);
        //    AppendElement(newDoc, postalAddress, "cbc:StreetName", nsCbc, WebsiteInfo.StreetName ?? "");
        //    AppendElement(newDoc, postalAddress, "cbc:BuildingNumber", nsCbc, WebsiteInfo.BuildingNumber ?? "");

        //    if (!string.IsNullOrEmpty(WebsiteInfo.PlotIdentification?.ToString()))
        //        AppendElement(newDoc, postalAddress, "cbc:PlotIdentification", nsCbc, WebsiteInfo.PlotIdentification.ToString());

        //    AppendElement(newDoc, postalAddress, "cbc:CitySubdivisionName", nsCbc, WebsiteInfo.CitySubdivisionName ?? "");
        //    AppendElement(newDoc, postalAddress, "cbc:CityName", nsCbc, WebsiteInfo.City ?? "");
        //    AppendElement(newDoc, postalAddress, "cbc:PostalZone", nsCbc, WebsiteInfo.PostalZone ?? "");

        //    XmlElement country = AppendContainerElement(newDoc, postalAddress, "cac:Country", nsCac);
        //    AppendElement(newDoc, country, "cbc:IdentificationCode", nsCbc, WebsiteIdentifyCRN?.Code ?? "SA");

        //    XmlElement partyTaxScheme = AppendContainerElement(newDoc, supplierParty, "cac:PartyTaxScheme", nsCac);
        //    AppendElement(newDoc, partyTaxScheme, "cbc:CompanyID", nsCbc, WebsiteIdentifyTAX?.Id ?? "");

        //    XmlElement taxScheme = AppendContainerElement(newDoc, partyTaxScheme, "cac:TaxScheme", nsCac);
        //    AppendElement(newDoc, taxScheme, "cbc:ID", nsCbc, "VAT");

        //    XmlElement partyLegalEntity = AppendContainerElement(newDoc, supplierParty, "cac:PartyLegalEntity", nsCac);
        //    AppendElement(newDoc, partyLegalEntity, "cbc:RegistrationName", nsCbc, WebsiteInfo.Title ?? "");
        //}

        ///// <summary>
        ///// Adds customer party information to XML
        ///// </summary>
        //[NonAction]
        //private void AddCustomerPartyToXml(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc, 
        //    ScSalesInvoice InvoiceInfo, string invoiceTypeCodeName, ShoppingCartConnection db)
        //{
        //    XmlElement customerNode = AppendContainerElement(newDoc, root, "cac:AccountingCustomerParty", nsCac);
        //    XmlElement customerParty = AppendContainerElement(newDoc, customerNode, "cac:Party", nsCac);

        //    string customerIdValue = "";
        //    string customerSchemeId = "CRN";
        //    string customerStreet = "";
        //    string customerBuilding = "";
        //    string customerSubdivision = "";
        //    string customerCity = "";
        //    string customerPostalZone = "";
        //    string customerCountryCode = "SA";
        //    string customerVatNumber = "";
        //    string customerName = "";

        //    // Get customer information if available
        //    if (InvoiceInfo.SupplierNumber != null)
        //    {
        //        var ClientInfo = db.ScClients.Where(x => x.AutoID == InvoiceInfo.SupplierNumber).SingleOrDefault();
        //        if (ClientInfo != null)
        //        {
        //            var CityName = db.ClsCities.Where(x => x.AutoID == ClientInfo.City).Select(x => x.Name).SingleOrDefault();
        //            if (ClientInfo.AccountNo != null)
        //            {
        //                var ClientIdentity = db.ScClientIdentifications.Where(x => x.ClientId == InvoiceInfo.SupplierNumber && (x.TypeId == 1 || x.TypeId == 2)).ToList();
        //                var ClientIdentifyCRN = ClientIdentity.FirstOrDefault(x => x.TypeId == 1);
        //                var ClientIdentifyTAX = ClientIdentity.FirstOrDefault(x => x.TypeId == 2);

        //                customerIdValue = ClientIdentifyCRN?.Id ?? "";
        //                customerSchemeId = ClientIdentifyCRN?.SchemeId ?? "CRN";
        //                customerStreet = ClientInfo.StreetName ?? "";
        //                customerBuilding = ClientInfo.BuildingNumber ?? "";
        //                customerSubdivision = ClientInfo.CitySubdivisionName ?? "";
        //                customerCity = CityName ?? "";
        //                customerPostalZone = ClientInfo.PostalZone ?? "";
        //                customerCountryCode = ClientIdentifyCRN?.Code ?? "SA";
        //                customerVatNumber = ClientIdentifyTAX?.Id ?? "";
        //                customerName = ClientInfo.Name ?? "";
        //            }
        //        }
        //    }

        //    XmlElement cusPartyIdentification = AppendContainerElement(newDoc, customerParty, "cac:PartyIdentification", nsCac);
        //    XmlElement customerId = AppendElement(newDoc, cusPartyIdentification, "cbc:ID", nsCbc, customerIdValue);
        //    customerId.SetAttribute("schemeID", customerSchemeId);

        //    XmlElement cusPostalAddress = AppendContainerElement(newDoc, customerParty, "cac:PostalAddress", nsCac);
        //    AppendElement(newDoc, cusPostalAddress, "cbc:StreetName", nsCbc, customerStreet);
        //    AppendElement(newDoc, cusPostalAddress, "cbc:BuildingNumber", nsCbc, customerBuilding);
        //    AppendElement(newDoc, cusPostalAddress, "cbc:CitySubdivisionName", nsCbc, customerSubdivision);
        //    AppendElement(newDoc, cusPostalAddress, "cbc:CityName", nsCbc, customerCity);
        //    AppendElement(newDoc, cusPostalAddress, "cbc:PostalZone", nsCbc, customerPostalZone);

        //    XmlElement cusCountry = AppendContainerElement(newDoc, cusPostalAddress, "cac:Country", nsCac);
        //    AppendElement(newDoc, cusCountry, "cbc:IdentificationCode", nsCbc, customerCountryCode);

        //    if (invoiceTypeCodeName == "0100000")
        //    {
        //        XmlElement cusPartyTaxScheme = AppendContainerElement(newDoc, customerParty, "cac:PartyTaxScheme", nsCac);

        //        if (!string.IsNullOrEmpty(customerVatNumber))
        //        {
        //            AppendElement(newDoc, cusPartyTaxScheme, "cbc:CompanyID", nsCbc, customerVatNumber);
        //        }

        //        XmlElement cusTaxScheme = AppendContainerElement(newDoc, cusPartyTaxScheme, "cac:TaxScheme", nsCac);
        //        AppendElement(newDoc, cusTaxScheme, "cbc:ID", nsCbc, "VAT");
        //    }

        //    XmlElement cusPartyLegalEntity = AppendContainerElement(newDoc, customerParty, "cac:PartyLegalEntity", nsCac);
        //    AppendElement(newDoc, cusPartyLegalEntity, "cbc:RegistrationName", nsCbc, customerName);
        //}

        /// <summary>
        /// Adds supplier party information to XML for GenerateXmlInvoice method
        /// </summary>
        [NonAction]
        private void AddSupplierPartyToXmlInvoice(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc, SupplierData supplier)
        {
            XmlElement supplierNode = AppendContainerElement(newDoc, root, "cac:AccountingSupplierParty", nsCac);
            XmlElement supplierParty = AppendContainerElement(newDoc, supplierNode, "cac:Party", nsCac);
            XmlElement partyIdentification = AppendContainerElement(newDoc, supplierParty, "cac:PartyIdentification", nsCac);

            XmlElement supplierId = AppendElement(newDoc, partyIdentification, "cbc:ID", nsCbc, supplier.IdNo);
            supplierId.SetAttribute("schemeID", supplier.IdType);

            XmlElement postalAddress = AppendContainerElement(newDoc, supplierParty, "cac:PostalAddress", nsCac);
            AppendElement(newDoc, postalAddress, "cbc:StreetName", nsCbc, supplier.StreetName);
            AppendElement(newDoc, postalAddress, "cbc:BuildingNumber", nsCbc, supplier.BuildingNo);
            AppendElement(newDoc, postalAddress, "cbc:PlotIdentification", nsCbc, supplier.PlotNumber);
            AppendElement(newDoc, postalAddress, "cbc:CitySubdivisionName", nsCbc, supplier.CitySubdivision);
            AppendElement(newDoc, postalAddress, "cbc:CityName", nsCbc, supplier.City);
            AppendElement(newDoc, postalAddress, "cbc:PostalZone", nsCbc, supplier.PostalCode);

            XmlElement country = AppendContainerElement(newDoc, postalAddress, "cac:Country", nsCac);
            AppendElement(newDoc, country, "cbc:IdentificationCode", nsCbc, "SA");

            XmlElement partyTaxScheme = AppendContainerElement(newDoc, supplierParty, "cac:PartyTaxScheme", nsCac);
            AppendElement(newDoc, partyTaxScheme, "cbc:CompanyID", nsCbc, supplier.VATNumber);

            XmlElement taxScheme = AppendContainerElement(newDoc, partyTaxScheme, "cac:TaxScheme", nsCac);
            AppendElement(newDoc, taxScheme, "cbc:ID", nsCbc, "VAT");

            XmlElement partyLegalEntity = AppendContainerElement(newDoc, supplierParty, "cac:PartyLegalEntity", nsCac);
            AppendElement(newDoc, partyLegalEntity, "cbc:RegistrationName", nsCbc, supplier.CompanName);
        }

        /// <summary>
        /// Adds customer party information to XML for GenerateXmlInvoice method
        /// </summary>
        [NonAction]
        private void AddCustomerPartyToXmlInvoice(XmlDocument newDoc, XmlElement root, string nsCac, string nsCbc, CustomerData customer, string invoiceTypeCodeName)
        {
            XmlElement customerNode = AppendContainerElement(newDoc, root, "cac:AccountingCustomerParty", nsCac);
            XmlElement customerParty = AppendContainerElement(newDoc, customerNode, "cac:Party", nsCac);

            XmlElement cusPartyIdentification = AppendContainerElement(newDoc, customerParty, "cac:PartyIdentification", nsCac);
            XmlElement customerId = AppendElement(newDoc, cusPartyIdentification, "cbc:ID", nsCbc, customer.BuyerIdNumber);
            customerId.SetAttribute("schemeID", customer.BuyerIdType);

            XmlElement cusPostalAddress = AppendContainerElement(newDoc, customerParty, "cac:PostalAddress", nsCac);
            AppendElement(newDoc, cusPostalAddress, "cbc:StreetName", nsCbc, customer.Street);
            AppendElement(newDoc, cusPostalAddress, "cbc:BuildingNumber", nsCbc, customer.BuildingNo);
            AppendElement(newDoc, cusPostalAddress, "cbc:CitySubdivisionName", nsCbc, customer.CitySubdivision);
            AppendElement(newDoc, cusPostalAddress, "cbc:CityName", nsCbc, customer.City);
            AppendElement(newDoc, cusPostalAddress, "cbc:PostalZone", nsCbc, customer.PostalCode);

            XmlElement cusCountry = AppendContainerElement(newDoc, cusPostalAddress, "cac:Country", nsCac);
            AppendElement(newDoc, cusCountry, "cbc:IdentificationCode", nsCbc, "SA");

            if (invoiceTypeCodeName == "0100000")
            {
                XmlElement cusPartyTaxScheme = AppendContainerElement(newDoc, customerParty, "cac:PartyTaxScheme", nsCac);

                if (!string.IsNullOrEmpty(customer.VATNumber))
                {
                    AppendElement(newDoc, cusPartyTaxScheme, "cbc:CompanyID", nsCbc, customer.VATNumber);
                }

                XmlElement cusTaxScheme = AppendContainerElement(newDoc, cusPartyTaxScheme, "cac:TaxScheme", nsCac);
                AppendElement(newDoc, cusTaxScheme, "cbc:ID", nsCbc, "VAT");
            }

            XmlElement cusPartyLegalEntity = AppendContainerElement(newDoc, customerParty, "cac:PartyLegalEntity", nsCac);
            AppendElement(newDoc, cusPartyLegalEntity, "cbc:RegistrationName", nsCbc, customer.PartyName);
        }

        /// <summary>
        /// Helper method to append attribute to an XML element
        /// </summary>
        [NonAction]
        public void AppendAttribute(XmlDocument newDoc, XmlElement parentNode, string attrName, string attrValue)
        {
            XmlAttribute attribute = newDoc.CreateAttribute(attrName);
            attribute.Value = attrValue;
            parentNode.Attributes.Append(attribute);
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

    #region Supporting Classes for GenerateXmlInvoice

    /// <summary>
    /// Invoice data container for full UBL invoice generation
    /// </summary>
    public class InvoiceData
    {
        public string Id { get; set; }
        public string InvoiceUUID { get; set; }
        public string IssueDate { get; set; }
        public string IssueTime { get; set; }
        public int InvoiceTypeCodeValue { get; set; }
        public string InvoiceTypeCodename { get; set; }
        public string InstructionNote { get; set; }
        public string Currency { get; set; }
        public string TaxCurrency { get; set; }
        public string PreviousInvoiceNumber { get; set; }
        public int ICV { get; set; }
        public string PIH { get; set; }
        public SupplierData AccountingSupplier { get; set; }
        public CustomerData AccountingCustomer { get; set; }
        public string ActualDeliveryDate { get; set; }
        public string LatestDeliveryDate { get; set; }
        public string PaymentMeansCode { get; set; }
        public string CreditDebitNoteReason { get; set; }
        public List<DocumentLevelAllowance> DocumentLevelAllowances { get; set; }
        public List<DocumentLevelCharge> DocumentLevelCharges { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public List<DocumentVatCategory> DocumentVatCategories { get; set; }
        public decimal LineExtensionAmount { get; set; }
        public decimal TaxExclusiveAmount { get; set; }
        public decimal TaxInclusiveAmount { get; set; }
        public decimal AllowanceTotalAmount { get; set; }
        public decimal ChargeTotalAmount { get; set; }
        public decimal PrepaidAmount { get; set; }
        public decimal PayableRoundAmount { get; set; }
        public decimal PayableAmount { get; set; }
        public List<InvoiceLineItemData> InvoiceLines { get; set; }
    }

    public class SupplierData
    {
        public string IdNo { get; set; }
        public string IdType { get; set; }
        public string StreetName { get; set; }
        public string BuildingNo { get; set; }
        public string PlotNumber { get; set; }
        public string CitySubdivision { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string VATNumber { get; set; }
        public string CompanName { get; set; }
    }

    public class CustomerData
    {
        public string BuyerIdNumber { get; set; }
        public string BuyerIdType { get; set; }
        public string Street { get; set; }
        public string BuildingNo { get; set; }
        public string CitySubdivision { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string VATNumber { get; set; }
        public string PartyName { get; set; }
    }

    public class DocumentLevelAllowance
    {
        public string AllowanceChargeReason { get; set; }
        public decimal Amount { get; set; }
        public string TaxCategoryCode { get; set; }
        public decimal TaxPercent { get; set; }
    }

    public class DocumentLevelCharge
    {
        public string AllowanceChargeReason { get; set; }
        public decimal Amount { get; set; }
        public string TaxCategoryCode { get; set; }
        public decimal TaxPercent { get; set; }
    }

    public class DocumentVatCategory
    {
        public decimal CatTaxableAmount { get; set; }
        public decimal CategoryVatAmount { get; set; }
        public string VatCategoryCode { get; set; }
        public decimal VatCategoryRate { get; set; }
        public string VatExCode { get; set; }
        public string VatExDesc { get; set; }
    }

    public class InvoiceLineItemData
    {
        public string LineID { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public string UnitCode { get; set; }
        public decimal LineExtensionAmount { get; set; }
        public string LineCurrency { get; set; }
        public List<LineLevelAllowance> LineLevelAllowances { get; set; }
        public List<LineLevelCharge> LineLevelCharges { get; set; }
        public string InvoiceLineType { get; set; }
        public string PrepaidInvoiceId { get; set; }
        public string PrepaidIssueDate { get; set; }
        public string PrepaidIssueTime { get; set; }
        public int PrepaidDocumentCode { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal RoundingAmount { get; set; }
        public decimal PrepaidTaxSubTotalTaxableAmount { get; set; }
        public decimal PrepaidTaxSubTotalTaxAmount { get; set; }
        public string TaxCategoryID { get; set; }
        public decimal TaxPercent { get; set; }
        public string ItemName { get; set; }
        public decimal PriceAmount { get; set; }
        public decimal BaseQuantity { get; set; }
        public string BaseUnitCode { get; set; }
    }

    public class LineLevelAllowance
    {
        public string Reason { get; set; }
        public decimal Amount { get; set; }
    }

    public class LineLevelCharge
    {
        public string Reason { get; set; }
        public decimal Amount { get; set; }
    }

    #endregion
}
