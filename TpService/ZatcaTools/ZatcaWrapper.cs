using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Zatca.EInvoice.SDK;
using Zatca.EInvoice.SDK.Contracts.Models;

namespace TpService.ZatcaTools
{
    [Guid("6FC70409-6E8B-4556-9C0B-D1146CA78C51")]
    public class ZatcaWrapper
    {
        
        private Utilities utils = new Utilities();

        public string LibraryVersion { get; set; } = "1.1.0";

        public ZatcaWrapper()
        {
          
        }

        public string GenerateInvoiceHash(string xmlInvoicePath)
        {
            var InvoiceHashGenerator = new Zatca.EInvoice.SDK.EInvoiceHashGenerator();
            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(xmlInvoicePath);

            HashResult Hashresult = InvoiceHashGenerator.GenerateEInvoiceHashing(xmlDoc);

            return Hashresult.Hash.ToString();
        }

        public void SignInvoice(string certificateContent, string privateKeyContent, string xmlInvoicePath, string signedXmlInvoicePath)
        {
            var InvoiceSigner = new Zatca.EInvoice.SDK.EInvoiceSigner();

            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(xmlInvoicePath);

            SignResult Signresult = InvoiceSigner.SignDocument(xmlDoc, certificateContent, privateKeyContent);

            Signresult.SaveSignedEInvoice(signedXmlInvoicePath);
        }

        //public void SignInvoiceJava(string certificateContent, string privateKeyContent, string xmlInvoicePath, string signedXmlInvoicePath, string sdkPath, string vatno)
        //{
        //    string inputfile = CleanXmlFileforJava(xmlInvoicePath);

        //    string tempFolderPath = Path.Combine(sdkPath, "temp");
        //    if (!Directory.Exists(tempFolderPath))
        //    {
        //        Directory.CreateDirectory(tempFolderPath);
        //    }

        //    string cert = Path.Combine(sdkPath, "temp", $"{vatno}_cert.pem");
        //    using (var writer = new StreamWriter(cert))
        //    {
        //        writer.Write(certificateContent);
        //    }

        //    string pvtkey = Path.Combine(sdkPath, "temp", $"{vatno}_pvtkey.pem");
        //    using (var writer = new StreamWriter(pvtkey))
        //    {
        //        writer.Write(privateKeyContent);
        //    }

        //    var javaSigner = new ZatcaJavaSigner(sdkPath, cert, signedXmlInvoicePath, pvtkey, inputfile);
        //    javaSigner.SignInvoice();
        //}

        //public string CleanXmlFileforJava(string invoiceFilePath)
        //{
        //    byte[] rawBytes = File.ReadAllBytes(invoiceFilePath);

        //    if (rawBytes.Length >= 3 && 
        //        rawBytes[0] == 0xEF && 
        //        rawBytes[1] == 0xBB && 
        //        rawBytes[2] == 0xBF)
        //    {
        //        rawBytes = rawBytes.Skip(3).ToArray();
        //    }

        //    string cleanedXml = Encoding.UTF8.GetString(rawBytes);

        //    string outfilePath = Path.Combine(Path.GetDirectoryName(invoiceFilePath), 
        //        Path.GetFileNameWithoutExtension(invoiceFilePath) + "_cleaned.xml");

        //    File.WriteAllText(outfilePath, cleanedXml, new UTF8Encoding(false));

        //    return outfilePath;
        //}

        public string GenerateQRString(string xmlInvoicefile)
        {
            var InvoiceQRGenerator = new Zatca.EInvoice.SDK.EInvoiceQRGenerator();

            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(xmlInvoicefile);

            QRResult QRresult = InvoiceQRGenerator.GenerateEInvoiceQRCode(xmlDoc);

            return QRresult.QR.ToString();
        }

        public string GenerateQRStringWithTime(string xmlInvoicefile)
        {
            string QRCodeString;

            var signedInvoice = new XmlDocument();
            signedInvoice.Load(xmlInvoicefile);
            var nsManager = new XmlNamespaceManager(signedInvoice.NameTable);
            nsManager.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            nsManager.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            nsManager.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");

            XmlNode qrNode = signedInvoice.SelectSingleNode("//cac:AdditionalDocumentReference[cbc:ID='QR']/cac:Attachment/cbc:EmbeddedDocumentBinaryObject", nsManager);

            if (qrNode != null)
            {
                QRCodeString = qrNode.InnerText;
            }
            else
            {
                QRCodeString = "";
            }

            return QRCodeString;
        }

        //public void GetQRImage(string qrCode, string imagePath)
        //{
        //    if (qrCode != null)
        //    {
        //        var qrcodeGenerator = new QRCodeGenerator();
        //        QRCodeGenerator.GenerateQRCode(qrCode, 500, 500, imagePath);
        //    }
        //}

        public GetKeyValues CSRgeneration(string commonName, string serialNo, string organizationIdentifier, 
            string organizationUnitName, string organizationName, string countryName, string invoiceType, 
            string locationAddress, string businessCategory, bool pemformat, string etype, string sdkpath)
        {
            var csrGenerator = new Zatca.EInvoice.SDK.CsrGenerator();
            var kCrypto = new GetKeyValues();

            var configObject = new CsrGenerationDto(commonName, serialNo, organizationIdentifier, 
                organizationUnitName, organizationName, countryName, invoiceType, locationAddress, businessCategory);

            EnvironmentType entype = EnvironmentType.Production;

            if (etype == "NonProduction")
            {
                entype = EnvironmentType.NonProduction;
            }
            else if (etype == "Simulation")
            {
                entype = EnvironmentType.Simulation;
            }
            else if (etype == "Production")
            {
                entype = EnvironmentType.Production;
            }

            CsrResult csrResult;

            if (etype == "Simulation")
            {
                //var zatcaWrapperJava = new FatooraCsrGenerator(commonName, serialNo, organizationIdentifier, 
                //    organizationUnitName, organizationName, countryName, invoiceType, locationAddress, 
                //    businessCategory, etype, sdkpath);
                //zatcaWrapperJava.GenerateCsr();

                //kCrypto.PvtKey = File.ReadAllText(zatcaWrapperJava.PrivateKeyFilePath);

                //var util = new Utilities();
                //kCrypto.CsrKey = File.ReadAllText(zatcaWrapperJava.CsrFilePath);
                //kCrypto.CsrKey = util.Base64DecodeString(kCrypto.CsrKey);
            }
            else
            {
                csrResult = csrGenerator.GenerateCsr(configObject, entype, pemformat);
                if (csrResult == null)
                {
                    kCrypto.PvtKey = "Error";
                    kCrypto.CsrKey = "CSR Returns Null";
                }
                else
                {
                    kCrypto.PvtKey = csrResult.PrivateKey.ToString();
                    kCrypto.CsrKey = csrResult.Csr.ToString();
                }
            }

            if (etype == "Production")
            {
                string pemString = Encoding.UTF8.GetString(Convert.FromBase64String(kCrypto.CsrKey));
                kCrypto.CsrKey = pemString;

                string base64Key = kCrypto.PvtKey;
                var lines = new List<string>();
                for (int i = 0; i < base64Key.Length; i += 64)
                {
                    int chunkLength = Math.Min(64, base64Key.Length - i);
                    lines.Add(base64Key.Substring(i, chunkLength));
                }

                string pemKey = "-----BEGIN EC PRIVATE KEY-----\r\n" +
                               string.Join("\r\n", lines) + "\r\n" +
                               "-----END EC PRIVATE KEY-----";

                kCrypto.PvtKey = pemKey;
            }

            return kCrypto;
        }

        public string ConvertPvtKeytoPem(string base64PvtKey)
        {
            string base64Key = base64PvtKey;
            var lines = new List<string>();
            for (int i = 0; i < base64Key.Length; i += 64)
            {
                int chunkLength = Math.Min(64, base64Key.Length - i);
                lines.Add(base64Key.Substring(i, chunkLength));
            }

            string pemKey = "-----BEGIN EC PRIVATE KEY-----\r\n" +
                           string.Join("\r\n", lines) + "\r\n" +
                           "-----END EC PRIVATE KEY-----";

            return pemKey;
        }

        public string ConvertCSRtoPem(string base64CSR)
        {
            string pemString = Encoding.UTF8.GetString(Convert.FromBase64String(base64CSR));
            return pemString;
        }

        private bool IsBase64String(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length % 4 != 0)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CSRgenerationFiles(string commonName, string serialNo, string organizationIdentifier, 
            string organizationUnitName, string organizationName, string countryName, string invoiceType, 
            string locationAddress, string businessCategory, bool pemformat, string etype, 
            string privatekeyFilePath, string csrFilePath)
        {
            var csrGenerator = new Zatca.EInvoice.SDK.CsrGenerator();

            var configObject = new CsrGenerationDto(commonName, serialNo, organizationIdentifier, 
                organizationUnitName, organizationName, countryName, invoiceType, locationAddress, businessCategory);

            EnvironmentType entype = EnvironmentType.Production;

            if (etype == "NonProduction")
            {
                entype = EnvironmentType.NonProduction;
            }
            else if (etype == "Simulation")
            {
                entype = EnvironmentType.Simulation;
            }
            else if (etype == "Production")
            {
                entype = EnvironmentType.Production;
            }

            CsrResult csrResult = csrGenerator.GenerateCsr(configObject, pemformat, entype);

            csrResult.SavePrivateKeyToFile(privatekeyFilePath);
            csrResult.SaveCsrToFile(csrFilePath);
        }

        public ComplianceCertificate GetComplianceCertificate(string TypeofCertificate, string token, string strCsr)
        {
            string cUrl = "";
            var complianceCertificate = new ComplianceCertificate();

            switch (TypeofCertificate)
            {
                case "NonProduction":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/developer-portal/compliance";
                    break;
                case "Simulation":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/simulation/compliance";
                    break;
                case "Production":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/core/compliance";
                    break;
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(strCsr);
            string Base64Csr = Convert.ToBase64String(plainTextBytes);

            string varQry = "{\"csr\":\"" + Base64Csr + "\"}";
            string mToken = token;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("OTP", mToken);
                client.DefaultRequestHeaders.Add("Accept-Version", "V2");

                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                var content = new StringContent(varQry, Encoding.UTF8, "application/json");
                var response = client.PostAsync(cUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    var ComplianceResponse = JsonConvert.DeserializeObject<CSIDResponse>(responseBody);

                    complianceCertificate.Certificate = ComplianceResponse.BinarySecurityToken;
                    complianceCertificate.Secret = ComplianceResponse.Secret;
                    complianceCertificate.Status = "Success";
                    complianceCertificate.RequestID = ComplianceResponse.RequestID.ToString();
                    complianceCertificate.Messages = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    complianceCertificate.Certificate = "";
                    complianceCertificate.Secret = "";
                    complianceCertificate.Status = "Error";
                    complianceCertificate.RequestID = "";
                    complianceCertificate.Messages = response.Content.ReadAsStringAsync().Result;
                }
            }

            return complianceCertificate;
        }

        public ProductionCertificate GetProductionCertificate(string TypeOfCertificate, string secret, 
            string CCSID, string ComplianceRequestId)
        {
            string cUrl = "";
            var ProductionCertificate = new ProductionCertificate();

            var zatcaUtils = new Utilities();
            if (zatcaUtils.IsInternetAvailable() == false)
            {
                return new ProductionCertificate
                {
                    Status = "Error",
                    Messages = "No internet connection available. Please check your internet connection and try again."
                };
            }

            switch (TypeOfCertificate)
            {
                case "NonProduction":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/developer-portal/production/csids";
                    break;
                case "Simulation":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/simulation/production/csids";
                    break;
                case "Production":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/core/production/csids";
                    break;
            }

            string varQry = "{\"compliance_request_id\":\"" + ComplianceRequestId + "\"}";
            string utfAuth = CCSID + ":" + secret;
            string varToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(utfAuth));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Version", "V2");
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + varToken);

                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                var content = new StringContent(varQry, Encoding.UTF8, "application/json");
                var response = client.PostAsync(cUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    var ProductionResponse = JsonConvert.DeserializeObject<CSIDResponse>(responseBody);

                    ProductionCertificate.Certificate = ProductionResponse.BinarySecurityToken;
                    ProductionCertificate.Secret = ProductionResponse.Secret;
                    ProductionCertificate.Status = "Success";
                    ProductionCertificate.Messages = responseBody;
                }
                else
                {
                    string ProductionBody = "Error: " + response.Content.ReadAsStringAsync().Result;
                    ProductionCertificate.Certificate = "";
                    ProductionCertificate.Secret = "";
                    ProductionCertificate.Status = "Error";
                    ProductionCertificate.Messages = ProductionBody;
                }
            }

            return ProductionCertificate;
        }

        public ZatcaSubmissionResponse ReportSimplified(string TypeOfCertificate, string invoiceHash, 
            string PCSID, string Secret, string signedInvoicePath, string invoiceUUID)
        {
            string JsonResponse;
            string cUrl = "";
            string statusCode;

         
            var zatcaUtils = new Utilities();

            if (zatcaUtils.IsInternetAvailable() == false)
            {
                return new ZatcaSubmissionResponse
                {
                    SubmissionStatus = "Error",
                    FullResponse = "No internet connection available. Please check your internet connection and try again.",
                    StatusCode = "503"
                };
            }
           

            switch (TypeOfCertificate)
            {
                case "NonProduction":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/developer-portal/invoices/reporting/single";
                    break;
                case "Simulation":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/simulation/invoices/reporting/single";
                    break;
                case "Production":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/core/invoices/reporting/single";
                    break;
            }

            string invsign = File.ReadAllText(signedInvoicePath, Encoding.UTF8);
            string b64Invoice = Convert.ToBase64String(Encoding.UTF8.GetBytes(invsign.Trim()));

            var jsonPayload = new Dictionary<string, string>
            {
                {"invoiceHash", invoiceHash},
                {"uuid", invoiceUUID},
                {"invoice", b64Invoice}
            };
            string jsonString = JsonConvert.SerializeObject(jsonPayload);

            string utfAuth = PCSID + ":" + Secret.Trim();
            string varToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(utfAuth));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Language", "en");
                client.DefaultRequestHeaders.Add("Accept-Version", "V2");
                client.DefaultRequestHeaders.Add("Clearance-Status", "1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", varToken);

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = client.PostAsync(cUrl, content).Result;

                statusCode = ((int)response.StatusCode).ToString();

                if ((int)response.StatusCode > 400)
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                    return new ZatcaSubmissionResponse
                    {
                        SubmissionStatus = "Error",
                        FullResponse = JsonResponse,
                        StatusCode = statusCode
                    };
                }
                else
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                }
            }

            var apiResponse = JsonConvert.DeserializeObject<Report>(JsonResponse);

            var zatcaResponse = new ZatcaSubmissionResponse();
            string warningMsg = "";
            int i = 1;

            foreach (var warning in apiResponse.ValidationResults.WarningMessages)
            {
                warningMsg += i.ToString() + ")" + warning.MessageText.ToString() + "\r\n";
                i++;
            }

            i = 1;
            string errorMsg = "";
            foreach (var errorm in apiResponse.ValidationResults.ErrorMessages)
            {
                errorMsg += i.ToString() + ")" + errorm.MessageText.ToString() + "\r\n";
                i++;
            }

            zatcaResponse.SubmissionStatus = apiResponse.ReportingStatus;
            zatcaResponse.FullResponse = JsonResponse;
            zatcaResponse.ZatcaWarnings = warningMsg;
            zatcaResponse.ZatcaErrors = errorMsg;
            zatcaResponse.StatusCode = statusCode;

            return zatcaResponse;
        }

        public string SignXmlInvoice(string Certificate, string pvtKey, string invoicePath, string signedInvoicePath)
        {
            string InvoiceHash;

            SignInvoice(Certificate, pvtKey, invoicePath, signedInvoicePath);
            InvoiceHash = GenerateInvoiceHash(signedInvoicePath);

            return InvoiceHash;
        }

        //public string SignXmlInvoiceJava(string Certificate, string pvtKey, string invoicePath, 
        //    string signedInvoicePath, string sdkPath, string vatno)
        //{
        //    string InvoiceHash;

        //    SignInvoiceJava(Certificate, pvtKey, invoicePath, signedInvoicePath, sdkPath, vatno);
        //    InvoiceHash = GenerateInvoiceHash(signedInvoicePath);

        //    return InvoiceHash;
        //}

        public ZatcaSubmissionResponse ClearStandard(string TypeOfCertificate, string invoiceHash, 
            string PCSID, string Secret, string signedInvoicePath, string invoiceUUID)
        {
            string JsonResponse;
            string cUrl = "";
            int statusCode;

          

            var zatcaUtils = new Utilities();
            if (zatcaUtils.IsInternetAvailable() == false)
            {
                return new ZatcaSubmissionResponse
                {
                    SubmissionStatus = "Error",
                    FullResponse = "No internet connection available. Please check your internet connection and try again.",
                    StatusCode = "503"
                };
            }
           

            switch (TypeOfCertificate)
            {
                case "NonProduction":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/developer-portal/invoices/clearance/single";
                    break;
                case "Simulation":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/simulation/invoices/clearance/single";
                    break;
                case "Production":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/core/invoices/clearance/single";
                    break;
            }

            string invsign = File.ReadAllText(signedInvoicePath, Encoding.UTF8);
            string b64Invoice = Convert.ToBase64String(Encoding.UTF8.GetBytes(invsign.Trim()));

            var jsonPayload = new Dictionary<string, string>
            {
                {"invoiceHash", invoiceHash},
                {"uuid", invoiceUUID},
                {"invoice", b64Invoice}
            };
            string jsonString = JsonConvert.SerializeObject(jsonPayload);

            string utfAuth = PCSID + ":" + Secret.Trim();
            string varToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(utfAuth));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Language", "en");
                client.DefaultRequestHeaders.Add("Accept-Version", "V2");
                client.DefaultRequestHeaders.Add("Clearance-Status", "1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", varToken);

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = client.PostAsync(cUrl, content).Result;

                statusCode = (int)response.StatusCode;

                if (statusCode > 400)
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                    return new ZatcaSubmissionResponse
                    {
                        SubmissionStatus = "Error",
                        FullResponse = JsonResponse,
                        StatusCode = statusCode.ToString(),
                        ZatcaWarnings = "",
                        ZatcaErrors = ""
                    };
                }
                else
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                }
            }

            var apiResponse = JsonConvert.DeserializeObject<Clearance>(JsonResponse);

            var zatcaResponse = new ZatcaSubmissionResponse();
            string ztcwarningMsg = "";
            int i = 1;

            foreach (var warning in apiResponse.ValidationResults.WarningMessages)
            {
                ztcwarningMsg += i.ToString() + ")" + warning.MessageText.ToString() + "\r\n";
                i++;
            }

            i = 1;
            string ztcerrorMsg = "";
            foreach (var errorm in apiResponse.ValidationResults.ErrorMessages)
            {
                ztcerrorMsg += i.ToString() + ")" + errorm.MessageText.ToString() + "\r\n";
                i++;
            }

            zatcaResponse.SubmissionStatus = apiResponse.ClearanceStatus;
            zatcaResponse.FullResponse = JsonResponse;

            if (apiResponse.ClearanceStatus == "CLEARED")
            {
                zatcaResponse.ClearedInvoice = zatcaUtils.Base64DecodeString(apiResponse.ClearedInvoice);
            }

            zatcaResponse.ZatcaWarnings = ztcwarningMsg;
            zatcaResponse.ZatcaErrors = ztcerrorMsg;
            zatcaResponse.StatusCode = statusCode.ToString();

            return zatcaResponse;
        }

        public ZatcaSubmissionResponse ClearStandardCompliance(string TypeOfCertificate, string invoiceHash, 
            string PCSID, string Secret, string signedInvoicePath, string invoiceUUID)
        {
            string JsonResponse;
            string cUrl = "";
            int statusCode;

            var zatcaUtils = new Utilities();
            if (zatcaUtils.IsInternetAvailable() == false)
            {
                return new ZatcaSubmissionResponse
                {
                    SubmissionStatus = "Error",
                    FullResponse = "No internet connection available. Please check your internet connection and try again.",
                    StatusCode = "503"
                };
            }

            switch (TypeOfCertificate)
            {
                case "NonProduction":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/developer-portal/compliance/invoices";
                    break;
                case "Simulation":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/simulation/compliance/invoices";
                    break;
                case "Production":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/core/compliance/invoices";
                    break;
            }

            string invsign = File.ReadAllText(signedInvoicePath, Encoding.UTF8);
            string b64Invoice = Convert.ToBase64String(Encoding.UTF8.GetBytes(invsign.Trim()));

            var jsonPayload = new Dictionary<string, string>
            {
                {"invoiceHash", invoiceHash},
                {"uuid", invoiceUUID},
                {"invoice", b64Invoice}
            };
            string jsonString = JsonConvert.SerializeObject(jsonPayload);

            string utfAuth = PCSID + ":" + Secret.Trim();
            string varToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(utfAuth));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Language", "en");
                client.DefaultRequestHeaders.Add("Accept-Version", "V2");
                client.DefaultRequestHeaders.Add("Clearance-Status", "1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", varToken);

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = client.PostAsync(cUrl, content).Result;

                statusCode = (int)response.StatusCode;

                if (statusCode > 400)
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                    return new ZatcaSubmissionResponse
                    {
                        SubmissionStatus = "Error",
                        FullResponse = JsonResponse,
                        StatusCode = statusCode.ToString()
                    };
                }
                else
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                }
            }

            var apiResponse = JsonConvert.DeserializeObject<Clearance>(JsonResponse);

            var zatcaResponse = new ZatcaSubmissionResponse();
            string ztcwarningMsg = "";
            int i = 1;

            foreach (var warning in apiResponse.ValidationResults.WarningMessages)
            {
                ztcwarningMsg += i.ToString() + ")" + warning.MessageText.ToString() + "\r\n";
                i++;
            }

            i = 1;
            string ztcerrorMsg = "";
            foreach (var errorm in apiResponse.ValidationResults.ErrorMessages)
            {
                ztcerrorMsg += i.ToString() + ")" + errorm.MessageText.ToString() + "\r\n";
                i++;
            }

            zatcaResponse.SubmissionStatus = apiResponse.ClearanceStatus;
            zatcaResponse.FullResponse = JsonResponse;
            zatcaResponse.ZatcaWarnings = ztcwarningMsg;
            zatcaResponse.ZatcaErrors = ztcerrorMsg;
            zatcaResponse.StatusCode = statusCode.ToString();

            return zatcaResponse;
        }

        public ZatcaSubmissionResponse ReportCompliance(string TypeOfCertificate, string invoiceHash, 
            string PCSID, string Secret, string signedInvoicePath, string invoiceUUID)
        {
            string JsonResponse;
            string cUrl = "";
            string statusCode;

            var zatcaUtils = new Utilities();
            if (zatcaUtils.IsInternetAvailable() == false)
            {
                return new ZatcaSubmissionResponse
                {
                    SubmissionStatus = "Error",
                    FullResponse = "No internet connection available. Please check your internet connection and try again.",
                    StatusCode = "503"
                };
            }

            switch (TypeOfCertificate)
            {
                case "NonProduction":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/developer-portal/compliance/invoices";
                    break;
                case "Simulation":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/simulation/compliance/invoices";
                    break;
                case "Production":
                    cUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/core/compliance/invoices";
                    break;
            }

            string invsign = File.ReadAllText(signedInvoicePath, Encoding.UTF8);
            string b64Invoice = Convert.ToBase64String(Encoding.UTF8.GetBytes(invsign.Trim()));

            var jsonPayload = new Dictionary<string, string>
            {
                {"invoiceHash", invoiceHash},
                {"uuid", invoiceUUID},
                {"invoice", b64Invoice}
            };
            string jsonString = JsonConvert.SerializeObject(jsonPayload);

            string utfAuth = PCSID + ":" + Secret.Trim();
            string varToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(utfAuth));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Language", "en");
                client.DefaultRequestHeaders.Add("Accept-Version", "V2");
                client.DefaultRequestHeaders.Add("Clearance-Status", "1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", varToken);

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = client.PostAsync(cUrl, content).Result;

                statusCode = ((int)response.StatusCode).ToString();

                if (int.Parse(statusCode) > 400)
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                    return new ZatcaSubmissionResponse
                    {
                        SubmissionStatus = "Error",
                        FullResponse = JsonResponse,
                        StatusCode = statusCode,
                        ZatcaWarnings = "",
                        ZatcaErrors = ""
                    };
                }
                else
                {
                    JsonResponse = response.Content.ReadAsStringAsync().Result;
                }
            }

            var apiResponse = JsonConvert.DeserializeObject<Report>(JsonResponse);

            var zatcaResponse = new ZatcaSubmissionResponse();
            string warningMsg = "";
            int i = 1;

            foreach (var warning in apiResponse.ValidationResults.WarningMessages)
            {
                warningMsg += i.ToString() + ")" + warning.MessageText.ToString() + "\r\n";
                i++;
            }

            i = 1;
            string errorMsg = "";
            foreach (var errorm in apiResponse.ValidationResults.ErrorMessages)
            {
                errorMsg += i.ToString() + ")" + errorm.MessageText.ToString() + "\r\n";
                i++;
            }

            zatcaResponse.SubmissionStatus = apiResponse.ReportingStatus;
            zatcaResponse.FullResponse = JsonResponse;
            zatcaResponse.ZatcaWarnings = warningMsg;
            zatcaResponse.ZatcaErrors = errorMsg;
            zatcaResponse.StatusCode = statusCode;

            return zatcaResponse;
        }
    }

    [Guid("7C941E5B-16D9-4665-B9EB-3CEE4FC26A80")]
    public class ZatcaSubmissionResponse
    {
        public string SubmissionStatus { get; set; }
        public List<Message> WarningMessages { get; set; }
        public List<Message> ErrorMessages { get; set; }
        public string ZatcaWarnings { get; set; }
        public string ZatcaErrors { get; set; }
        public string FullResponse { get; set; }
        public string ClearedInvoice { get; set; }
        public string StatusCode { get; set; }
        public string ClearedQRCode { get; set; }

        public string GetClearedQrCode()
        {
            string qrCode = "";
            string clearedInvoice = this.ClearedInvoice;
            if (clearedInvoice != null)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(clearedInvoice);

                var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                nsManager.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                nsManager.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");

                XmlNode qrNode = xmlDoc.SelectSingleNode("//cac:AdditionalDocumentReference[cbc:ID='QR']/cac:Attachment/cbc:EmbeddedDocumentBinaryObject", nsManager);

                if (qrNode != null)
                {
                    qrCode = qrNode.InnerText;
                }
            }
            return qrCode;
        }

        public void GetQRImage(string imagePath)
        {
            string qrCode = GetClearedQrCode();
            if (qrCode != null)
            {
                var qrcodeGenerator = new QRCodeGenerator();
                QRCodeGenerator.GenerateQRCode(qrCode, 500, 500, imagePath);
            }
        }
    }

    [Guid("49CDF2BE-AB7C-42D0-B460-D8E20D8DF9B5")]
    public class ProductionCertificate
    {
        public string Certificate { get; set; }
        public string Secret { get; set; }
        public string Status { get; set; }
        public string Messages { get; set; }
        public string RequestID { get; set; }
    }

    public class ComplianceCertificate
    {
        public string Certificate { get; set; }
        public string Secret { get; set; }
        public string Status { get; set; }
        public string Messages { get; set; }
        public string RequestID { get; set; }
    }

    public class CSIDResponse
    {
        [JsonProperty("requestID")]
        public long RequestID { get; set; }

        [JsonProperty("dispositionMessage")]
        public string DispositionMessage { get; set; }

        [JsonProperty("binarySecurityToken")]
        public string BinarySecurityToken { get; set; }

        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }
    }

    [Guid("A38DC16A-3F55-4E7E-B927-0BB998ED98E4")]
    public class GetKeyValues
    {
        public string PvtKey { get; set; }
        public string CsrKey { get; set; }
    }

    [Guid("383BCF5B-0D3A-4D62-A50C-A943ED49025C")]
    public class Utilities
    {
        public string Base64EncodeString(string textValue)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(textValue);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string Base64DecodeString(string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string GetGuid()
        {
            Guid id = Guid.NewGuid();
            return id.ToString();
        }

        public bool IsInternetAvailable()
        {
            try
            {
                string[] servers = { "8.8.8.8", "8.8.4.4", "1.1.1.1", "www.google.com" };
                int timeout = 2000;

                var pingResults = servers.AsParallel().Any(server =>
                {
                    try
                    {
                        using (var pingSender = new Ping())
                        {
                            PingReply reply = pingSender.Send(server, timeout);
                            if (reply.Status == IPStatus.Success)
                            {
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error pinging {server}: {ex.Message}");
                    }
                    return false;
                });

                if (pingResults)
                {
                    return true;
                }

                try
                {
                    using (var client = new WebClient())
                    {
                        using (var stream = client.OpenRead("http://www.google.com"))
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking internet availability: " + ex.Message);
                return false;
            }
        }

        public void LogToFile(string message)
        {
            try
            {
                string logFilePath = "zatcalogfile.txt";
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

                using (var writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error logging message: " + ex.Message);
            }
        }

        public bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }

            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public string GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "No IPv4 address found";
        }
    }

    public class Message
    {
        public string Code { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        [JsonProperty("message")]
        public string MessageText { get; set; }
    }

    public class Report
    {
        [JsonProperty("reportingStatus")]
        public string ReportingStatus { get; set; }

        [JsonProperty("validationResults")]
        public ValidationResult ValidationResults { get; set; }
    }

    public class Clearance
    {
        [JsonProperty("clearanceStatus")]
        public string ClearanceStatus { get; set; }

        [JsonProperty("clearedInvoice")]
        public string ClearedInvoice { get; set; }

        [JsonProperty("validationResults")]
        public ValidationResult ValidationResults { get; set; }
    }

    //public class ValidationResults
    //{
    //    [JsonProperty("warningMessages")]
    //    public List<Message> WarningMessages { get; set; }

    //    [JsonProperty("errorMessages")]
    //    public List<Message> ErrorMessages { get; set; }
    //}


    public class ValidationResult
    {
        public List<Message> InfoMessages { get; set; }
        public List<Message> WarningMessages { get; set; }
        public List<Message> ErrorMessages { get; set; }
        public string Status { get; set; }
    }

}
