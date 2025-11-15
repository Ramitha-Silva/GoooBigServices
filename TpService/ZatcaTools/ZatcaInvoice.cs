using System;
using System.Collections.Generic;

using System.Data;

using System.Text;
using System.Xml;

namespace TpService.ZatcaTools
{
    
    public class ZatcaInvoice
    {
        public string Id { get; set; }
        public string InvoiceUUID { get; set; }
        public string InvoiceTypeCodename { get; set; }
        public string InvoiceCategory { get; set; }
        public string InvoiceTypeCodeValue { get; set; }
        public string InvoiceType { get; set; }
        public int ICV { get; set; }
        public string PIH { get; set; }
        public string InstructionNote { get; set; }
        public string IssueDate { get; set; }
        public string IssueTime { get; set; }
        public string Currency { get; set; }
        public string TaxCurrency { get; set; }
        public string ActualDeliveryDate { get; set; }
        public string LatestDeliveryDate { get; set; }
        public string PaymentMeansCode { get; set; }
        public string TotalTaxableAmount { get; set; }
        public string TaxTotalAmount { get; set; }
        public string TaxCategoryCode { get; set; }
        public string VATRate { get; set; }
        public string LineExtensionAmount { get; set; }
        public string TaxExclusiveAmount { get; set; }
        public string TaxInclusiveAmount { get; set; }
        public string AllowanceTotalAmount { get; set; }
        public string ChargeTotalAmount { get; set; }
        public string PayableRoundAmount { get; set; }
        public string PrepaidAmount { get; set; } = "0.00";
        public string PayableAmount { get; set; }
        public string TotalAmount { get; set; }
        public string TotalTaxAmount { get; set; }
        public Supplier AccountingSupplier { get; set; }
        public Customer AccountingCustomer { get; set; }
        public List<InvoiceLineItem> InvoiceLines { get; set; }
        public List<DocumentLevelAllowance> DocumentLevelAllowances { get; set; }
        public List<DocumentLevelCharge> DocumentLevelCharges { get; set; }
        public string PreviousInvoiceNumber { get; set; }
        public string CreditDebitNoteReason { get; set; }
        public string InvoiceCreationStatusCode { get; set; }
        public string InvoiceCreationStatus { get; set; }
        public string TaxExcemptioReasonCode { get; set; }
        public string TaxExcemptionReason { get; set; }
        public string LibraryVersion { get; set; } = "1.2.0";
        
        public List<UniqeTaxCategory> DocumentVatCategories { get; set; }

        public ZatcaInvoice()
        {
        }

        public ZatcaInvoice(string invoiceUUID, string id, string invoiceCategory, string invoiceType,
            int icv, string pih, string instructionNote, string issueDate, string issueTime,
            string currency, string taxCurrency, string actualDeliveryDate, string latestDeliveryDate,
            string paymentMeansCode, string taxCategoryCode, string vatRate,
            Supplier accountingSupplier = null, Customer accountingCustomer = null,
            List<InvoiceLineItem> invoiceLines = null, List<DocumentLevelAllowance> documentLevelAllowances = null,
            List<DocumentLevelCharge> documentLevelCharges = null, string previousInvoiceNumber = "",
            string creditDebitNoteReason = "")
        {
            this.Id = id;
            this.InvoiceUUID = invoiceUUID;

            if (invoiceCategory == "SIMPLIFIED")
            {
                this.InvoiceTypeCodename = "0200000";
            }
            else if (invoiceCategory == "TAXINVOICE")
            {
                this.InvoiceTypeCodename = "0100000";
            }
            else
            {
                this.InvoiceCreationStatusCode = "FAILED";
                this.InvoiceCreationStatus = "Valid Invoice Category was not provided";
            }

            if (invoiceType == "INVOICE")
            {
                this.InvoiceTypeCodeValue = "388";
            }
            else if (invoiceType == "CREDITNOTE")
            {
                this.InvoiceTypeCodeValue = "381";
            }
            else if (invoiceType == "DEBITNOTE")
            {
                this.InvoiceTypeCodeValue = "383";
            }
            else if (invoiceType == "PREPAYMENT")
            {
                this.InvoiceTypeCodeValue = "386";
            }
            else
            {
                this.InvoiceCreationStatusCode = "FAILED";
                this.InvoiceCreationStatus = "Valid Invoice Type was not provided";
            }

            this.InvoiceCategory = invoiceCategory;
            this.InvoiceType = invoiceType;
            this.ICV = icv;
            this.PIH = pih;
            this.InstructionNote = instructionNote;
            this.IssueDate = issueDate;
            this.IssueTime = issueTime;
            this.Currency = currency;
            this.TaxCurrency = taxCurrency;
            this.ActualDeliveryDate = actualDeliveryDate;
            this.LatestDeliveryDate = latestDeliveryDate;
            this.PaymentMeansCode = paymentMeansCode;
            this.TaxCategoryCode = taxCategoryCode;
            this.VATRate = vatRate;
            this.PreviousInvoiceNumber = previousInvoiceNumber;
            this.CreditDebitNoteReason = creditDebitNoteReason;

            this.AccountingSupplier = accountingSupplier ?? new Supplier();
            this.AccountingCustomer = accountingCustomer ?? new Customer();
            this.InvoiceLines = invoiceLines ?? new List<InvoiceLineItem>();
            this.DocumentLevelAllowances = documentLevelAllowances ?? new List<DocumentLevelAllowance>();
            this.DocumentLevelCharges = documentLevelCharges ?? new List<DocumentLevelCharge>();

            this.InvoiceCreationStatusCode = "SUCCESS";
        }

        public void PopulateInvoice(string invoiceUUID, string id, string invoiceCategory, string invoiceType,
            int icv, string pih, string instructionNote, string issueDate, string issueTime,
            string currency, string taxCurrency, string actualDeliveryDate, string latestDeliveryDate,
            string paymentMeansCode, string taxCategoryCode, string vatRate, string previousInvoiceNumber = "",
            string creditDebitNoteReason = "", Supplier accountingSupplier = null, Customer accountingCustomer = null,
            List<InvoiceLineItem> invoiceLines = null, List<DocumentLevelAllowance> documentLevelAllowances = null,
            List<DocumentLevelCharge> documentLevelCharges = null)
        {
            this.Id = id;
            this.InvoiceUUID = invoiceUUID;

            if (invoiceCategory == "SIMPLIFIED")
            {
                this.InvoiceTypeCodename = "0200000";
            }
            else if (invoiceCategory == "TAXINVOICE")
            {
                this.InvoiceTypeCodename = "0100000";
            }
            else
            {
                this.InvoiceCreationStatusCode = "FAILED";
                this.InvoiceCreationStatus = "Valid Invoice Category was not provided";
            }

            if (invoiceType == "INVOICE")
            {
                this.InvoiceTypeCodeValue = "388";
            }
            else if (invoiceType == "CREDITNOTE")
            {
                this.InvoiceTypeCodeValue = "381";
            }
            else if (invoiceType == "DEBITNOTE")
            {
                this.InvoiceTypeCodeValue = "383";
            }
            else if (invoiceType == "PREPAYMENT")
            {
                this.InvoiceTypeCodeValue = "386";
            }
            else
            {
                this.InvoiceCreationStatusCode = "FAILED";
                this.InvoiceCreationStatus = "Valid Invoice Type was not provided";
            }

            this.InvoiceCategory = invoiceCategory;
            this.InvoiceType = invoiceType;
            this.ICV = icv;
            this.PIH = pih;
            this.InstructionNote = instructionNote;
            this.IssueDate = issueDate;
            this.IssueTime = issueTime;
            this.Currency = currency;
            this.TaxCurrency = taxCurrency;
            this.ActualDeliveryDate = actualDeliveryDate;
            this.LatestDeliveryDate = latestDeliveryDate;
            this.PaymentMeansCode = paymentMeansCode;
            this.TaxCategoryCode = taxCategoryCode;
            this.VATRate = vatRate;
            this.PreviousInvoiceNumber = previousInvoiceNumber;
            this.CreditDebitNoteReason = creditDebitNoteReason;

            this.AccountingSupplier = accountingSupplier ?? new Supplier();
            this.AccountingCustomer = accountingCustomer ?? new Customer();
            this.InvoiceLines = invoiceLines ?? new List<InvoiceLineItem>();
            this.DocumentLevelAllowances = documentLevelAllowances ?? new List<DocumentLevelAllowance>();
            this.DocumentLevelCharges = documentLevelCharges ?? new List<DocumentLevelCharge>();

            this.InvoiceCreationStatusCode = "SUCCESS";
        }

        public void SaveXmlInvoice(string xmlPath)
        {
            XmlDocument updatedInvoice = this.GenerateXml();
            updatedInvoice.Save(xmlPath);
        }

        public XmlDocument GenerateXml()
        {
            XmlDocument newDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            newDoc.AppendChild(xmlDeclaration);

            string ns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
            string nsCac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            string nsCbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
            string nsExt = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

            XmlElement root = newDoc.CreateElement("Invoice", ns);
            root.SetAttribute("xmlns", ns);
            root.SetAttribute("xmlns:cac", nsCac);
            root.SetAttribute("xmlns:cbc", nsCbc);
            root.SetAttribute("xmlns:ext", nsExt);
            newDoc.AppendChild(root);

            AppendElement(newDoc, root, "cbc:ProfileID", nsCbc, "reporting:1.0");
            AppendElement(newDoc, root, "cbc:ID", nsCbc, Id);
            AppendElement(newDoc, root, "cbc:UUID", nsCbc, InvoiceUUID);
            AppendElement(newDoc, root, "cbc:IssueDate", nsCbc, IssueDate);
            AppendElement(newDoc, root, "cbc:IssueTime", nsCbc, IssueTime);

            XmlElement invoiceTypeCodeElement = AppendElement(newDoc, root, "cbc:InvoiceTypeCode", nsCbc, InvoiceTypeCodeValue);
            invoiceTypeCodeElement.SetAttribute("name", InvoiceTypeCodename);

            XmlElement invoiceNote = AppendElement(newDoc, root, "cbc:Note", nsCbc, InstructionNote);
            invoiceNote.SetAttribute("languageID", "ar");

            AppendElement(newDoc, root, "cbc:DocumentCurrencyCode", nsCbc, Currency);
            AppendElement(newDoc, root, "cbc:TaxCurrencyCode", nsCbc, TaxCurrency);

            if (InvoiceTypeCodeValue == "381" || InvoiceTypeCodeValue == "383")
            {
                XmlElement billingReferenceNode = AppendContainerElement(newDoc, root, "cac:BillingReference", nsCac);
                XmlElement invoiceDocumentReferenceNode = AppendContainerElement(newDoc, billingReferenceNode, "cac:InvoiceDocumentReference", nsCac);
                AppendElement(newDoc, invoiceDocumentReferenceNode, "cbc:ID", nsCbc, PreviousInvoiceNumber);
            }

            XmlElement icvRef = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
            AppendElement(newDoc, icvRef, "cbc:ID", nsCbc, "ICV");
            AppendElement(newDoc, icvRef, "cbc:UUID", nsCbc, ICV.ToString());

            XmlElement pihRef = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
            AppendElement(newDoc, pihRef, "cbc:ID", nsCbc, "PIH");
            XmlElement attachment = AppendContainerElement(newDoc, pihRef, "cac:Attachment", nsCac);
            if (string.IsNullOrEmpty(PIH))
            {
                PIH = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";
            }
            XmlElement pihBinaryObject = AppendElement(newDoc, attachment, "cbc:EmbeddedDocumentBinaryObject", nsCbc, PIH);
            pihBinaryObject.SetAttribute("mimeCode", "text/plain");

            XmlElement additionalDocumentReferenceNode = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac);
            AppendElement(newDoc, additionalDocumentReferenceNode, "cbc:ID", nsCbc, "QR");
            XmlElement attachmentNode = AppendContainerElement(newDoc, additionalDocumentReferenceNode, "cac:Attachment", nsCac);
            XmlElement qrBinaryObject = AppendElement(newDoc, attachmentNode, "cbc:EmbeddedDocumentBinaryObject", nsCbc, "ARNBY21lIFdpZGdldOKAmXMgTFREAg8zMTExMTExMTExMDExMTMDFDIwMjEtMDEtMDVUMDk6MzI6NDBaBAYyNTAuMDAFBDAuMDAGLG5CNTVSbENQajFhRjFkcC9zUkcrVTI2aGc4UDA2TTlCdFRNeHNlb2N5ckU9B2BNRVlDSVFDNFZvZlJxdlZJUng3VUoxTzl4Vjl3SjVFUTdBZk5UV3BnTFdEWEhpSlZZd0loQUw1S040TDR6WUhrMnVwWjlOWmUxYjJwK0pldWtTcEM2OXMyZ284ZXNBaVoIWDBWMBAGByqGSM49AgEGBSuBBAAKA0IABGGDDKDmhWAITDv7LXqLX2cmr6+qddUkpcLCvWs5rC2O29W/hS4ajAK4Qdnahym6MaijX75Cg3j4aao7ouYXJ9EJRjBEAiA6CM08lbTXuWwiKOZVBWQ/sbMU7YpAp30Ydq6QuAhYWwIgUkX27AqFMzEONZs37VrCycUjtEsFHED/qFn4XXC1qpQ=");
            qrBinaryObject.SetAttribute("mimeCode", "text/plain");

            XmlElement signatureNode = AppendContainerElement(newDoc, root, "cac:Signature", nsCac);
            AppendElement(newDoc, signatureNode, "cbc:ID", nsCbc, "urn:oasis:names:specification:ubl:signature:Invoice");
            AppendElement(newDoc, signatureNode, "cbc:SignatureMethod", nsCbc, "urn:oasis:names:specification:ubl:dsig:enveloped:xades");

            XmlElement supplierNode = AppendContainerElement(newDoc, root, "cac:AccountingSupplierParty", nsCac);
            XmlElement supplierParty = AppendContainerElement(newDoc, supplierNode, "cac:Party", nsCac);
            XmlElement partyIdentification = AppendContainerElement(newDoc, supplierParty, "cac:PartyIdentification", nsCac);
            AppendElement(newDoc, partyIdentification, "cbc:ID", nsCbc, AccountingSupplier.IdNo).SetAttribute("schemeID", AccountingSupplier.IdType);

            XmlElement postalAddress = AppendContainerElement(newDoc, supplierParty, "cac:PostalAddress", nsCac);
            AppendElement(newDoc, postalAddress, "cbc:StreetName", nsCbc, AccountingSupplier.StreetName);
            AppendElement(newDoc, postalAddress, "cbc:BuildingNumber", nsCbc, AccountingSupplier.BuildingNo);
            AppendElement(newDoc, postalAddress, "cbc:PlotIdentification", nsCbc, AccountingSupplier.PlotNumber);
            AppendElement(newDoc, postalAddress, "cbc:CitySubdivisionName", nsCbc, AccountingSupplier.CitySubdivision);
            AppendElement(newDoc, postalAddress, "cbc:CityName", nsCbc, AccountingSupplier.City);
            AppendElement(newDoc, postalAddress, "cbc:PostalZone", nsCbc, AccountingSupplier.PostalCode);

            XmlElement country = AppendContainerElement(newDoc, postalAddress, "cac:Country", nsCac);
            AppendElement(newDoc, country, "cbc:IdentificationCode", nsCbc, "SA");

            XmlElement partyTaxScheme = AppendContainerElement(newDoc, supplierParty, "cac:PartyTaxScheme", nsCac);
            AppendElement(newDoc, partyTaxScheme, "cbc:CompanyID", nsCbc, AccountingSupplier.VATNumber);

            XmlElement taxScheme = AppendContainerElement(newDoc, partyTaxScheme, "cac:TaxScheme", nsCac);
            AppendElement(newDoc, taxScheme, "cbc:ID", nsCbc, "VAT");

            XmlElement partyLegalEntity = AppendContainerElement(newDoc, supplierParty, "cac:PartyLegalEntity", nsCac);
            AppendElement(newDoc, partyLegalEntity, "cbc:RegistrationName", nsCbc, AccountingSupplier.CompanName);

            XmlElement customerNode = AppendContainerElement(newDoc, root, "cac:AccountingCustomerParty", nsCac);
            XmlElement customerParty = AppendContainerElement(newDoc, customerNode, "cac:Party", nsCac);

            XmlElement cusPartyIdentification = AppendContainerElement(newDoc, customerParty, "cac:PartyIdentification", nsCac);
            AppendElement(newDoc, cusPartyIdentification, "cbc:ID", nsCbc, AccountingCustomer.BuyerIdNumber).SetAttribute("schemeID", AccountingCustomer.BuyerIdType);

            XmlElement cusPostalAddress = AppendContainerElement(newDoc, customerParty, "cac:PostalAddress", nsCac);
            AppendElement(newDoc, cusPostalAddress, "cbc:StreetName", nsCbc, AccountingCustomer.Street);
            AppendElement(newDoc, cusPostalAddress, "cbc:BuildingNumber", nsCbc, AccountingCustomer.BuildingNo);
            AppendElement(newDoc, cusPostalAddress, "cbc:CitySubdivisionName", nsCbc, AccountingCustomer.CitySubdivision);
            AppendElement(newDoc, cusPostalAddress, "cbc:CityName", nsCbc, AccountingCustomer.City);
            AppendElement(newDoc, cusPostalAddress, "cbc:PostalZone", nsCbc, AccountingCustomer.PostalCode);

            XmlElement cusCountry = AppendContainerElement(newDoc, cusPostalAddress, "cac:Country", nsCac);
            AppendElement(newDoc, cusCountry, "cbc:IdentificationCode", nsCbc, "SA");

            if (InvoiceTypeCodename == "0100000")
            {
                XmlElement cusPartyTaxScheme = AppendContainerElement(newDoc, customerParty, "cac:PartyTaxScheme", nsCac);

                if (!string.IsNullOrEmpty(AccountingCustomer.VATNumber))
                {
                    AppendElement(newDoc, cusPartyTaxScheme, "cbc:CompanyID", nsCbc, AccountingCustomer.VATNumber);
                }

                XmlElement cusTaxScheme = AppendContainerElement(newDoc, cusPartyTaxScheme, "cac:TaxScheme", nsCac);
                AppendElement(newDoc, cusTaxScheme, "cbc:ID", nsCbc, "VAT");
            }

            XmlElement cusPartyLegalEntity = AppendContainerElement(newDoc, customerParty, "cac:PartyLegalEntity", nsCac);
            AppendElement(newDoc, cusPartyLegalEntity, "cbc:RegistrationName", nsCbc, AccountingCustomer.PartyName);

            XmlElement deliveryNode = AppendContainerElement(newDoc, root, "cac:Delivery", nsCac);
            AppendElement(newDoc, deliveryNode, "cbc:ActualDeliveryDate", nsCbc, ActualDeliveryDate);
            AppendElement(newDoc, deliveryNode, "cbc:LatestDeliveryDate", nsCbc, LatestDeliveryDate);

            XmlElement paymentMeansNode = AppendContainerElement(newDoc, root, "cac:PaymentMeans", nsCac);
            AppendElement(newDoc, paymentMeansNode, "cbc:PaymentMeansCode", nsCbc, PaymentMeansCode);

            if (InvoiceTypeCodeValue == "381" || InvoiceTypeCodeValue == "383")
            {
                AppendElement(newDoc, paymentMeansNode, "cbc:InstructionNote", nsCbc, CreditDebitNoteReason);
            }

            if (DocumentLevelAllowances != null)
            {
                foreach (var allowance in DocumentLevelAllowances)
                {
                    XmlElement allowanceChargeNode = AppendContainerElement(newDoc, root, "cac:AllowanceCharge", nsCac);
                    AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "false");
                    AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, allowance.AllowanceChargeReason);
                    AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, allowance.Amount.ToString()).SetAttribute("currencyID", "SAR");

                    XmlElement taxCategoryNode = AppendContainerElement(newDoc, allowanceChargeNode, "cac:TaxCategory", nsCac);
                    XmlElement taxCategoryIdElement1 = newDoc.CreateElement("cbc:ID", nsCbc);
                    taxCategoryIdElement1.InnerText = allowance.TaxCategoryCode;
                    taxCategoryIdElement1.SetAttribute("schemeID", "UN/ECE 5305");
                    taxCategoryIdElement1.SetAttribute("schemeAgencyID", "6");
                    taxCategoryNode.AppendChild(taxCategoryIdElement1);

                    AppendElement(newDoc, taxCategoryNode, "cbc:Percent", nsCbc, allowance.TaxPercent.ToString());

                    XmlElement taxSchemeNode = AppendContainerElement(newDoc, taxCategoryNode, "cac:TaxScheme", nsCac);
                    XmlElement taxSchemeIdElement = newDoc.CreateElement("cbc:ID", nsCbc);
                    taxSchemeIdElement.InnerText = "VAT";
                    taxSchemeIdElement.SetAttribute("schemeID", "UN/ECE 5153");
                    taxSchemeIdElement.SetAttribute("schemeAgencyID", "6");
                    taxSchemeNode.AppendChild(taxSchemeIdElement);
                }
            }

            if (DocumentLevelCharges != null)
            {
                foreach (var charges in DocumentLevelCharges)
                {
                    XmlElement allowanceChargeNode = AppendContainerElement(newDoc, root, "cac:AllowanceCharge", nsCac);
                    AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true");
                    AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReasonCode", nsCbc, "CG");
                    AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, charges.AllowanceChargeReason);
                    AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, charges.Amount.ToString()).SetAttribute("currencyID", "SAR");

                    XmlElement taxCategoryNode = AppendContainerElement(newDoc, allowanceChargeNode, "cac:TaxCategory", nsCac);
                    XmlElement taxCategoryIdElement2 = newDoc.CreateElement("cbc:ID", nsCbc);
                    taxCategoryIdElement2.InnerText = charges.TaxCategoryCode;
                    taxCategoryIdElement2.SetAttribute("schemeID", "UN/ECE 5305");
                    taxCategoryIdElement2.SetAttribute("schemeAgencyID", "6");
                    taxCategoryNode.AppendChild(taxCategoryIdElement2);

                    AppendElement(newDoc, taxCategoryNode, "cbc:Percent", nsCbc, charges.TaxPercent.ToString());

                    XmlElement taxSchemeNode = AppendContainerElement(newDoc, taxCategoryNode, "cac:TaxScheme", nsCac);
                    XmlElement taxSchemeIdElement = newDoc.CreateElement("cbc:ID", nsCbc);
                    taxSchemeIdElement.InnerText = "VAT";
                    taxSchemeIdElement.SetAttribute("schemeID", "UN/ECE 5153");
                    taxSchemeIdElement.SetAttribute("schemeAgencyID", "6");
                    taxSchemeNode.AppendChild(taxSchemeIdElement);
                }
            }

            XmlElement taxTotalNode1 = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac);
            AppendElement(newDoc, taxTotalNode1, "cbc:TaxAmount", nsCbc, TotalTaxAmount).SetAttribute("currencyID", "SAR");

            XmlElement taxTotalNode = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac);
            AppendElement(newDoc, taxTotalNode, "cbc:TaxAmount", nsCbc, TotalTaxAmount).SetAttribute("currencyID", "SAR");

            foreach (var taxcat in DocumentVatCategories)
            {
                XmlElement taxSubtotalNode = AppendContainerElement(newDoc, taxTotalNode, "cac:TaxSubtotal", nsCac);
                AppendElement(newDoc, taxSubtotalNode, "cbc:TaxableAmount", nsCbc, taxcat.CatTaxableAmount).SetAttribute("currencyID", "SAR");

                if (taxcat.VatCategoryCode != "S")
                {
                    AppendElement(newDoc, taxSubtotalNode, "cbc:TaxAmount", nsCbc, "0.00").SetAttribute("currencyID", "SAR");
                }
                else
                {
                    AppendElement(newDoc, taxSubtotalNode, "cbc:TaxAmount", nsCbc, taxcat.CategoryVatAmount).SetAttribute("currencyID", "SAR");
                }

                XmlElement taxCategoryInSubtotalNode = AppendContainerElement(newDoc, taxSubtotalNode, "cac:TaxCategory", nsCac);
                XmlElement taxCategoryIdElement = newDoc.CreateElement("cbc:ID", nsCbc);
                taxCategoryIdElement.InnerText = taxcat.VatCategoryCode;
                taxCategoryIdElement.SetAttribute("schemeID", "UN/ECE 5305");
                taxCategoryIdElement.SetAttribute("schemeAgencyID", "6");
                taxCategoryInSubtotalNode.AppendChild(taxCategoryIdElement);

                if (taxcat.VatCategoryCode != "S")
                {
                    AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:Percent", nsCbc, "0.00");
                }
                else
                {
                    AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:Percent", nsCbc, taxcat.VatCategoryRate);
                }

                if (taxcat.VatCategoryCode == "Z" || taxcat.VatCategoryCode == "E")
                {
                    TaxExcemptioReasonCode = taxcat.VatExCode;
                    TaxExcemptionReason = taxcat.VatExDesc;
                    AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:TaxExemptionReasonCode", nsCbc, TaxExcemptioReasonCode);
                    AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:TaxExemptionReason", nsCbc, TaxExcemptionReason);
                }

                XmlElement taxSchemeInSubtotalNode = AppendContainerElement(newDoc, taxCategoryInSubtotalNode, "cac:TaxScheme", nsCac);
                var element = AppendElement(newDoc, taxSchemeInSubtotalNode, "cbc:ID", nsCbc, "VAT");
                element.SetAttribute("schemeID", "UN/ECE 5153");
                element.SetAttribute("schemeAgencyID", "6");
            }

            XmlElement legalMonetaryTotalNode = AppendContainerElement(newDoc, root, "cac:LegalMonetaryTotal", nsCac);
            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:LineExtensionAmount", nsCbc, LineExtensionAmount).SetAttribute("currencyID", "SAR");
            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxExclusiveAmount", nsCbc, TaxExclusiveAmount).SetAttribute("currencyID", "SAR");
            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxInclusiveAmount", nsCbc, TaxInclusiveAmount).SetAttribute("currencyID", "SAR");

            if (decimal.Parse(AllowanceTotalAmount) > 0)
            {
                AppendElement(newDoc, legalMonetaryTotalNode, "cbc:AllowanceTotalAmount", nsCbc, AllowanceTotalAmount).SetAttribute("currencyID", "SAR");
            }

            if (decimal.Parse(ChargeTotalAmount) > 0)
            {
                AppendElement(newDoc, legalMonetaryTotalNode, "cbc:ChargeTotalAmount", nsCbc, ChargeTotalAmount).SetAttribute("currencyID", "SAR");
            }

            if (decimal.Parse(PrepaidAmount) > 0)
            {
                AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PrepaidAmount", nsCbc, PrepaidAmount).SetAttribute("currencyID", "SAR");
            }

            if (decimal.Parse(PayableRoundAmount) != 0)
            {
                AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PayableRoundingAmount", nsCbc, PayableRoundAmount).SetAttribute("currencyID", "SAR");
            }

            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PayableAmount", nsCbc, PayableAmount).SetAttribute("currencyID", "SAR");

            if (InvoiceLines != null)
            {
                foreach (var invoiceLine in InvoiceLines)
                {
                    XmlElement invoiceLineNode = AppendContainerElement(newDoc, root, "cac:InvoiceLine", nsCac);
                    AppendElement(newDoc, invoiceLineNode, "cbc:ID", nsCbc, invoiceLine.LineID);

                    XmlElement invoicedQuantityNode = AppendElement(newDoc, invoiceLineNode, "cbc:InvoicedQuantity", nsCbc, invoiceLine.InvoicedQuantity);
                    invoicedQuantityNode.SetAttribute("unitCode", invoiceLine.UnitCode);

                    AppendElement(newDoc, invoiceLineNode, "cbc:LineExtensionAmount", nsCbc, invoiceLine.LineExtensionAmount).SetAttribute("currencyID", invoiceLine.LineCurrency);

                    if (InvoiceTypeCodename == "0100000")
                    {
                        if (invoiceLine.LineLevelAllowances != null)
                        {
                            foreach (var allowance in invoiceLine.LineLevelAllowances)
                            {
                                XmlElement allowanceChargeNode1 = AppendContainerElement(newDoc, invoiceLineNode, "cac:AllowanceCharge", nsCac);
                                AppendElement(newDoc, allowanceChargeNode1, "cbc:ChargeIndicator", nsCbc, "false");
                                AppendElement(newDoc, allowanceChargeNode1, "cbc:AllowanceChargeReason", nsCbc, allowance.Reason);
                                AppendElement(newDoc, allowanceChargeNode1, "cbc:Amount", nsCbc, allowance.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency);
                            }
                        }

                        if (invoiceLine.LineLevelCharges != null)
                        {
                            foreach (var charge in invoiceLine.LineLevelCharges)
                            {
                                XmlElement allowanceChargeNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:AllowanceCharge", nsCac);
                                AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true");
                                AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, charge.Reason);
                                AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, charge.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency);
                            }
                        }
                    }

                    if (invoiceLine.InvoiceLineType == "PREPAID")
                    {
                        XmlElement prepaidDocumentReferenceNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:DocumentReference", nsCac);
                        AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:ID", nsCbc, invoiceLine.PrepaidInvoiceId);
                        AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:IssueDate", nsCbc, invoiceLine.PrepaidIssueDate);
                        AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:IssueTime", nsCbc, invoiceLine.PrepaidIssueTime);
                        AppendElement(newDoc, prepaidDocumentReferenceNode, "cbc:DocumentTypeCode", nsCbc, invoiceLine.PrepaidDocumentCode);
                    }

                    XmlElement lineTaxTotalNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:TaxTotal", nsCac);
                    AppendElement(newDoc, lineTaxTotalNode, "cbc:TaxAmount", nsCbc, invoiceLine.TaxAmount).SetAttribute("currencyID", "SAR");
                    AppendElement(newDoc, lineTaxTotalNode, "cbc:RoundingAmount", nsCbc, invoiceLine.RoundingAmount).SetAttribute("currencyID", "SAR");

                    if (invoiceLine.InvoiceLineType == "PREPAID")
                    {
                        XmlElement prepaidTaxSubtotalNode = AppendContainerElement(newDoc, lineTaxTotalNode, "cac:TaxSubtotal", nsCac);
                        AppendElement(newDoc, prepaidTaxSubtotalNode, "cbc:TaxableAmount", nsCbc, invoiceLine.PrepaidTaxSubTotalTaxableAmount).SetAttribute("currencyID", "SAR");
                        AppendElement(newDoc, prepaidTaxSubtotalNode, "cbc:TaxAmount", nsCbc, invoiceLine.PrepaidTaxSubTotalTaxAmount).SetAttribute("currencyID", "SAR");

                        XmlElement prepaidTaxCategoryInSubtotalNode = AppendContainerElement(newDoc, prepaidTaxSubtotalNode, "cac:TaxCategory", nsCac);
                        XmlElement prepaidTaxCategoryIdElement = newDoc.CreateElement("cbc:ID", nsCbc);
                        prepaidTaxCategoryIdElement.InnerText = invoiceLine.TaxCategoryID;
                        prepaidTaxCategoryInSubtotalNode.AppendChild(prepaidTaxCategoryIdElement);

                        AppendElement(newDoc, prepaidTaxCategoryInSubtotalNode, "cbc:Percent", nsCbc, invoiceLine.TaxPercent);

                        XmlElement prepaidTaxSchemeInSubtotalNode = AppendContainerElement(newDoc, prepaidTaxCategoryInSubtotalNode, "cac:TaxScheme", nsCac);
                        AppendElement(newDoc, prepaidTaxSchemeInSubtotalNode, "cbc:ID", nsCbc, "VAT");
                    }

                    XmlElement itemNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:Item", nsCac);
                    AppendElement(newDoc, itemNode, "cbc:Name", nsCbc, invoiceLine.ItemName);

                    XmlElement classifiedTaxCategoryNode = AppendContainerElement(newDoc, itemNode, "cac:ClassifiedTaxCategory", nsCac);
                    AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:ID", nsCbc, invoiceLine.TaxCategoryID);
                    AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:Percent", nsCbc, invoiceLine.TaxPercent);

                    XmlElement taxSchemeNode = AppendContainerElement(newDoc, classifiedTaxCategoryNode, "cac:TaxScheme", nsCac);
                    AppendElement(newDoc, taxSchemeNode, "cbc:ID", nsCbc, "VAT");

                    XmlElement priceNode = AppendContainerElement(newDoc, invoiceLineNode, "cac:Price", nsCac);
                    AppendElement(newDoc, priceNode, "cbc:PriceAmount", nsCbc, invoiceLine.PriceAmount).SetAttribute("currencyID", invoiceLine.LineCurrency);

                    if (invoiceLine.InvoiceLineType != "PREPAID")
                    {
                        AppendElement(newDoc, priceNode, "cbc:BaseQuantity", nsCbc, invoiceLine.BaseQuantity).SetAttribute("unitCode", invoiceLine.BaseUnitCode);
                    }

                    if (InvoiceTypeCodename == "0200000")
                    {
                        if (invoiceLine.LineLevelAllowances != null)
                        {
                            foreach (var allowance in invoiceLine.LineLevelAllowances)
                            {
                                XmlElement allowanceChargeNode1 = AppendContainerElement(newDoc, priceNode, "cac:AllowanceCharge", nsCac);
                                AppendElement(newDoc, allowanceChargeNode1, "cbc:ChargeIndicator", nsCbc, "false");
                                AppendElement(newDoc, allowanceChargeNode1, "cbc:AllowanceChargeReason", nsCbc, allowance.Reason);
                                AppendElement(newDoc, allowanceChargeNode1, "cbc:Amount", nsCbc, allowance.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency);
                            }
                        }

                        if (invoiceLine.LineLevelCharges != null)
                        {
                            foreach (var charge in invoiceLine.LineLevelCharges)
                            {
                                XmlElement allowanceChargeNode = AppendContainerElement(newDoc, priceNode, "cac:AllowanceCharge", nsCac);
                                AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true");
                                AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, charge.Reason);
                                AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, charge.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency);
                            }
                        }
                    }
                }
            }

            return newDoc;
        }

        private XmlElement AppendElement(XmlDocument doc, XmlElement parent, string name, string ns, string value)
        {
            XmlElement element = doc.CreateElement(name, ns);
            element.InnerText = value;
            parent.AppendChild(element);
            return element;
        }

        private XmlElement AppendContainerElement(XmlDocument doc, XmlElement parent, string name, string ns)
        {
            XmlElement element = doc.CreateElement(name, ns);
            parent.AppendChild(element);
            return element;
        }

        public void AppendAttribute(XmlDocument newDoc, XmlElement parentNode, string attrName, string attrValue)
        {
            XmlAttribute attribute = newDoc.CreateAttribute(attrName);
            attribute.Value = attrValue;
            parentNode.Attributes.Append(attribute);
        }

        public void AddInvoiceLineItem(string invoiceLineNo, string quantity, string unitCode,
            string itemName, string taxCategoryID, string taxPercent, string itemPrice, string priceCurrency,
            string baseQuantity, string baseUnitCode, string vatExCode = "", string vatExDesc = "",
            List<LineLevelAllowance> lineLevelAllowances = null, List<LineLevelCharge> lineLevelCharges = null,
            string invoiceLineType = "REGULAR", string prepaidAmount = "0")
        {
            

            var invoiceLine = new InvoiceLineItem(invoiceLineNo, quantity, unitCode, itemName, taxCategoryID,
                taxPercent, itemPrice, priceCurrency, baseQuantity, baseUnitCode, InvoiceTypeCodename,
                vatExCode, vatExDesc);

            InvoiceLines.Add(invoiceLine);
            RecalculateDocument();
        }

        public void AddLineLevelCharges(string line, string reason, string amount)
        {
            var foundInvoiceLine = InvoiceLines.Find(item => item.LineID == line);
            foundInvoiceLine.AddLineLevelCharge(reason, amount);
            RecalculateDocument();
        }

        public void AddLineLevelAllowance(string line, string reason, string amount)
        {
            var foundInvoiceLine = InvoiceLines.Find(item => item.LineID == line);
            foundInvoiceLine.AddLineLevelAllowance(reason, amount);
            RecalculateDocument();
        }

        public DataTable GetInvoiceLines()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LineID", typeof(string));
            dt.Columns.Add("LineExtensionAmt", typeof(string));
            dt.Columns.Add("LineTaxAmt", typeof(string));
            dt.Columns.Add("LineNetAmt", typeof(string));
            dt.Columns.Add("LineAllowanceCharge", typeof(string));

            foreach (var line in InvoiceLines)
            {
                dt.Rows.Add(line.LineID, line.LineExtensionAmount, line.TaxAmount, line.LineNetAmount, line.AllowanceChargeAmount);
            }

            return dt;
        }

        public string GetInvoiceLinesAsCSV()
        {
            DataTable dt = GetInvoiceLines();
            StringBuilder csv = new StringBuilder();

            foreach (DataColumn column in dt.Columns)
            {
                csv.Append(column.ColumnName + ",");
            }
            csv.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    csv.Append(row[col].ToString() + ",");
                }
                csv.AppendLine();
            }

            return csv.ToString();
        }

        public void AddCustomer(string partyName, string vatNumber, string idType, string idNumber,
            string buildingNo, string street, string citySubdivision, string city, string postalCode)
        {
            var customer = new Customer(partyName, vatNumber, idType, idNumber, buildingNo, street,
                citySubdivision, city, postalCode);
            this.AccountingCustomer = customer;
        }

        public void AddSupplier(string idType, string idNo, string companName, string vatNumber,
            string streetName, string buildingNo, string plotNumber, string citySubdivision,
            string city, string postalCode)
        {
            var supplier = new Supplier(idType, idNo, companName, vatNumber, streetName, buildingNo,
                plotNumber, citySubdivision, city, postalCode);
            this.AccountingSupplier = supplier;
        }

        public void AddInvoiceLineItem(ref InvoiceLineItem invoiceLine)
        {
            
            invoiceLine.InvoiceTypeCodename = this.InvoiceTypeCodename;
            invoiceLine.ReCalculateLine();
            InvoiceLines.Add(invoiceLine);
            RecalculateDocument();
        }

        public void AddPrepaymentAmount(string prepaidAmount, string quantity, string unitCode, string itemName,
            string taxCategoryID, string taxPercent, string priceAmount, string prepayCurrency, string baseQuantity,
            string baseUnitCode, string prepaidInvoiceId, string prepaidInvoiceIssueDate, string prepaidInvoiceIssueTime,
            List<LineLevelAllowance> lineLevelAllowances = null, List<LineLevelCharge> lineLevelCharges = null)
        {
            decimal tempPrepaid = 0;
            foreach (var line in InvoiceLines)
            {
                if (line.InvoiceLineType == "PREPAID")
                {
                    tempPrepaid += decimal.Parse(line.PrepaidAmount);
                }
            }

            this.PrepaidAmount = (tempPrepaid + decimal.Parse(prepaidAmount)).ToString();

            string newInvoiceLine = (InvoiceLines.Count + 1).ToString();

            var invoiceLine = new InvoiceLineItem(newInvoiceLine, quantity, unitCode, "Prepayment adjustment",
                taxCategoryID, taxPercent, "0", prepayCurrency, "1", baseUnitCode, InvoiceTypeCodename, "", "",
                null, null, "PREPAID", prepaidAmount);
            invoiceLine.PrepaidDocumentCode = "386";
            invoiceLine.PrepaidInvoiceId = prepaidInvoiceId;
            invoiceLine.PrepaidIssueDate = prepaidInvoiceIssueDate;
            invoiceLine.PrepaidIssueTime = prepaidInvoiceIssueTime;

            InvoiceLines.Add(invoiceLine);
            RecalculateDocument();
        }

        public void AddDocumentLevelAllowance(string reason, decimal amount, decimal taxPercent, string taxCategoryCode)
        {
            DocumentLevelAllowances.Add(new DocumentLevelAllowance(reason, amount, taxPercent, taxCategoryCode));
            RecalculateDocument();
        }

        public void AddDocumentLevelCharge(string reason, decimal amount, decimal taxPercent, string taxCategoryCode)
        {
            DocumentLevelCharges.Add(new DocumentLevelCharge(reason, amount, taxPercent, taxCategoryCode));
            RecalculateDocument();
        }

        private void RecalculateDocument()
        {
            decimal tempLineExtensionAmount = 0;
            decimal tempAllowanceTotalAmount = 0;
            decimal tempChargeTotalAmount = 0;
            decimal tempTotalTaxableAmount = 0;
            decimal tempTotalTaxAmount = 0;
            decimal tempTaxExclusiveAmount = 0;
            decimal tempTaxInclusiveAmount = 0;
            decimal tempPayableAmount = 0;
            decimal tempVATRate = 0;
            decimal tempPrepaidAmount = 0;
            decimal lineNetPrice = 0;
            decimal totalLineTaxAmount = 0;
            decimal tempCatVatAmount = 0;
            decimal tempCatTaxableAmount = 0;

            this.DocumentVatCategories = new List<UniqeTaxCategory>();

            tempVATRate = Convert.ToDecimal(this.VATRate);
            tempPrepaidAmount = Convert.ToDecimal(this.PrepaidAmount);

            if (InvoiceLines != null)
            {
                foreach (var line in InvoiceLines)
                {
                    tempLineExtensionAmount += Convert.ToDecimal(line.LineExtensionAmount);
                    lineNetPrice += (Convert.ToDecimal(line.ItemPrice) * Convert.ToDecimal(line.InvoicedQuantity)) + Convert.ToDecimal(line.AllowanceChargeAmount);
                    totalLineTaxAmount += decimal.Parse(line.TaxAmount);

                    if (line.TaxCategoryID == "S")
                    {
                        var foundItem = DocumentVatCategories.Find(x => x.VatCategoryCode == "S");
                        if (foundItem != null)
                        {
                            tempCatVatAmount = Convert.ToDecimal(foundItem.CategoryVatAmount);
                            tempCatVatAmount += decimal.Parse(line.TaxAmount);
                            foundItem.CategoryVatAmount = tempCatVatAmount.ToString();

                            tempCatTaxableAmount = Convert.ToDecimal(foundItem.CatTaxableAmount);
                            tempCatTaxableAmount += Convert.ToDecimal(line.LineExtensionAmount);
                            foundItem.CatTaxableAmount = tempCatTaxableAmount.ToString();
                        }
                        else
                        {
                            var vc = new UniqeTaxCategory
                            {
                                VatCategoryCode = "S",
                                CategoryVatAmount = line.TaxAmount,
                                VatCategoryRate = line.TaxPercent,
                                CatTaxableAmount = line.LineExtensionAmount
                            };
                            DocumentVatCategories.Add(vc);
                        }
                    }

                    if (line.TaxCategoryID == "E")
                    {
                        var foundItem = DocumentVatCategories.Find(x => x.VatCategoryCode == "E");
                        if (foundItem != null)
                        {
                            foundItem.CategoryVatAmount = "0.00";
                            tempCatTaxableAmount = Convert.ToDecimal(foundItem.CatTaxableAmount);
                            tempCatTaxableAmount += Convert.ToDecimal(line.LineExtensionAmount);
                            foundItem.CatTaxableAmount = tempCatTaxableAmount.ToString();
                        }
                        else
                        {
                            var vc = new UniqeTaxCategory
                            {
                                VatCategoryCode = "E",
                                CategoryVatAmount = "0.00",
                                CatTaxableAmount = line.LineExtensionAmount,
                                VatExCode = line.VatExCode,
                                VatExDesc = line.VatExDescription
                            };
                            DocumentVatCategories.Add(vc);
                        }
                    }

                    if (line.TaxCategoryID == "Z")
                    {
                        var foundItem = DocumentVatCategories.Find(x => x.VatCategoryCode == "Z");
                        if (foundItem != null)
                        {
                            foundItem.CategoryVatAmount = "0.00";
                            tempCatTaxableAmount = Convert.ToDecimal(foundItem.CatTaxableAmount);
                            tempCatTaxableAmount += Convert.ToDecimal(line.LineExtensionAmount);
                            foundItem.CatTaxableAmount = tempCatTaxableAmount.ToString();
                        }
                        else
                        {
                            var vc = new UniqeTaxCategory
                            {
                                VatCategoryCode = "Z",
                                CategoryVatAmount = "0.00",
                                CatTaxableAmount = line.LineExtensionAmount,
                                VatExCode = line.VatExCode,
                                VatExDesc = line.VatExDescription
                            };
                            DocumentVatCategories.Add(vc);
                        }
                    }
                }
            }

            if (DocumentLevelAllowances != null)
            {
                foreach (var docAllowance in DocumentLevelAllowances)
                {
                    tempAllowanceTotalAmount += docAllowance.Amount;
                }
            }

            if (DocumentLevelCharges != null)
            {
                foreach (var docCharge in DocumentLevelCharges)
                {
                    tempChargeTotalAmount += docCharge.Amount;
                }
            }

            foreach (var cat in DocumentVatCategories)
            {
                decimal tempChargeTotalAmountForCat = 0;
                if (DocumentLevelCharges != null)
                {
                    foreach (var docCharge in DocumentLevelCharges)
                    {
                        if (docCharge.TaxCategoryCode == cat.VatCategoryCode)
                        {
                            tempChargeTotalAmountForCat += docCharge.Amount;
                        }
                    }
                }

                decimal tempAllowanceTotalAmountForCat = 0;
                if (DocumentLevelAllowances != null)
                {
                    foreach (var docAllowance in DocumentLevelAllowances)
                    {
                        if (docAllowance.TaxCategoryCode == cat.VatCategoryCode)
                        {
                            tempAllowanceTotalAmountForCat += docAllowance.Amount;
                        }
                    }
                }

                decimal categoryVatAmount = (tempChargeTotalAmountForCat - tempAllowanceTotalAmountForCat) * decimal.Parse(cat.VatCategoryRate) / 100;
                cat.CategoryVatAmount = Math.Round(decimal.Parse(cat.CategoryVatAmount) + categoryVatAmount, 2, MidpointRounding.AwayFromZero).ToString();
                cat.CatTaxableAmount = (decimal.Parse(cat.CatTaxableAmount) - tempAllowanceTotalAmountForCat + tempChargeTotalAmountForCat).ToString();

                if (cat.VatCategoryCode == "S")
                {
                    tempTotalTaxAmount += decimal.Parse(cat.CategoryVatAmount);
                }

                tempTotalTaxableAmount += decimal.Parse(cat.CatTaxableAmount);
            }

            lineNetPrice = lineNetPrice - tempAllowanceTotalAmount + tempChargeTotalAmount;
            tempTaxExclusiveAmount = tempLineExtensionAmount - tempAllowanceTotalAmount + tempChargeTotalAmount;
            tempTaxInclusiveAmount = tempTaxExclusiveAmount + tempTotalTaxAmount;

            decimal tempPayableRoundingAmount;
            if (this.InvoiceTypeCodename == "0200000")
            {
                tempPayableRoundingAmount = Math.Round(lineNetPrice - tempTaxInclusiveAmount, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                tempPayableRoundingAmount = 0;
            }

            tempPayableAmount = tempTaxInclusiveAmount - tempPrepaidAmount + tempPayableRoundingAmount;

            this.LineExtensionAmount = tempLineExtensionAmount.ToString("F2");
            this.AllowanceTotalAmount = tempAllowanceTotalAmount.ToString("F2");
            this.ChargeTotalAmount = tempChargeTotalAmount.ToString("F2");
            this.TotalTaxableAmount = tempTotalTaxableAmount.ToString("F2");
            this.TotalTaxAmount = tempTotalTaxAmount.ToString("F2");
            this.TaxExclusiveAmount = tempTaxExclusiveAmount.ToString("F2");
            this.TaxInclusiveAmount = tempTaxInclusiveAmount.ToString("F2");
            this.PayableAmount = tempPayableAmount.ToString("F2");
            this.PayableRoundAmount = tempPayableRoundingAmount.ToString("F2");
        }

       
        public class Customer
        {
            public string PartyName { get; set; }
            public string VATNumber { get; set; }
            public string BuyerIdType { get; set; }
            public string BuyerIdNumber { get; set; }
            public string BuildingNo { get; set; }
            public string Street { get; set; }
            public string CitySubdivision { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }

            public Customer()
            {
            }

            public Customer(string partyName, string vatNumber, string idType, string idNumber,
                string buildingNo, string street, string citySubdivision, string city, string postalCode)
            {
                this.PartyName = partyName;
                this.VATNumber = vatNumber;
                this.BuyerIdType = idType;
                this.BuyerIdNumber = idNumber;
                this.BuildingNo = buildingNo;
                this.Street = street;
                this.CitySubdivision = citySubdivision;
                this.PostalCode = postalCode;
                this.City = city;
            }

            public void PopulateCustomer(string partyName, string vatNumber, string idType, string idNumber,
                string buildingNo, string street, string citySubdivision, string city, string postalCode)
            {
                this.PartyName = partyName;
                this.VATNumber = vatNumber;
                this.BuyerIdType = idType;
                this.BuyerIdNumber = idNumber;
                this.BuildingNo = buildingNo;
                this.Street = street;
                this.CitySubdivision = citySubdivision;
                this.PostalCode = postalCode;
                this.City = city;
            }
        }

        
        public class Supplier
        {
            public string IdType { get; set; }
            public string IdNo { get; set; }
            public string CompanName { get; set; }
            public string VATNumber { get; set; }
            public string StreetName { get; set; }
            public string BuildingNo { get; set; }
            public string PlotNumber { get; set; }
            public string CitySubdivision { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }

            public Supplier()
            {
            }

            public Supplier(string idType, string idNo, string companName, string vatNumber,
                string streetName, string buildingNo, string plotNumber, string citySubdivision,
                string city, string postalCode)
            {
                this.IdType = idType;
                this.IdNo = idNo;
                this.CompanName = companName;
                this.VATNumber = vatNumber;
                this.StreetName = streetName;
                this.BuildingNo = buildingNo;
                this.PlotNumber = plotNumber;
                this.CitySubdivision = citySubdivision;
                this.City = city;
                this.PostalCode = postalCode;
            }

            public void PopulateSupplier(string idType, string idNo, string companName, string vatNumber,
                string streetName, string buildingNo, string plotNumber, string citySubdivision,
                string city, string postalCode)
            {
                this.IdType = idType;
                this.IdNo = idNo;
                this.CompanName = companName;
                this.VATNumber = vatNumber;
                this.StreetName = streetName;
                this.BuildingNo = buildingNo;
                this.PlotNumber = plotNumber;
                this.CitySubdivision = citySubdivision;
                this.City = city;
                this.PostalCode = postalCode;
            }
        }

       
        public class DocumentLevelAllowance
        {
            public string AllowanceChargeReason { get; set; }
            public decimal Amount { get; set; }
            public decimal TaxPercent { get; set; }
            public string TaxCategoryCode { get; set; }

            public DocumentLevelAllowance()
            {
            }

            public DocumentLevelAllowance(string allowanceReason, decimal amount, decimal taxPercent, string taxCategoryCode)
            {
                this.AllowanceChargeReason = allowanceReason;
                this.Amount = amount;
                this.TaxPercent = taxPercent;
                this.TaxCategoryCode = taxCategoryCode;
            }
        }

       
        public class DocumentLevelCharge
        {
            public string AllowanceChargeReason { get; set; }
            public decimal Amount { get; set; }
            public decimal TaxPercent { get; set; }
            public string TaxCategoryCode { get; set; }

            public DocumentLevelCharge(string chargeReason, decimal amount, decimal taxPercent, string taxCategoryCode)
            {
                this.AllowanceChargeReason = chargeReason;
                this.Amount = amount;
                this.TaxPercent = taxPercent;
                this.TaxCategoryCode = taxCategoryCode;
            }
        }

      
       

       
        
    }



    public class LineLevelCharge
    {
        public string Reason { get; set; }
        public string Amount { get; set; }

        public LineLevelCharge(string reason, string amount)
        {
            this.Reason = reason;
            this.Amount = amount;
        }
    }

    public class LineLevelAllowance
    {
        public string Reason { get; set; }
        public string Amount { get; set; }

        public LineLevelAllowance(string reason, string amount)
        {
            this.Reason = reason;
            this.Amount = amount;
        }
    }


    public class InvoiceLineItem
    {
        public string LineID { get; set; }
        public string InvoicedQuantity { get; set; }
        public string UnitCode { get; set; }
        public string LineExtensionAmount { get; set; }
        public string TaxAmount { get; set; }
        public string RoundingAmount { get; set; }
        public string ItemName { get; set; }
        public string TaxCategoryID { get; set; }
        public string TaxPercent { get; set; }
        public string ItemPrice { get; set; }
        public string LineCurrency { get; set; }
        public string PriceAmount { get; set; }
        public string AllowanceChargeAmount { get; set; }
        public string BaseQuantity { get; set; }
        public string BaseUnitCode { get; set; }
        public List<LineLevelCharge> LineLevelCharges { get; set; }
        public List<LineLevelAllowance> LineLevelAllowances { get; set; }
        public string InvoiceLineType { get; set; } = "REGULAR";
        public string PrepaidAmount { get; set; }
        public string PrepaidInvoiceId { get; set; }
        public string PrepaidIssueDate { get; set; }
        public string PrepaidIssueTime { get; set; }
        public string PrepaidDocumentCode { get; set; }
        public string PrepaidTaxSubTotalTaxableAmount { get; set; }
        public string PrepaidTaxSubTotalTaxAmount { get; set; }
        public string LineNetAmount { get; set; }
        public string VatExCode { get; set; }
        public string VatExDescription { get; set; }
        public string InvoiceTypeCodename { get; set; }
        public string LibraryVersion { get; set; } = "1.1.0";

        public InvoiceLineItem()
        {
        }

        public InvoiceLineItem(string invoiceLineNo, string quantity, string unitCode, string itemName,
            string taxCategoryID, string taxPercent, string itemPrice, string priceCurrency, string baseQuantity,
            string baseUnitCode, string invoiceTypeCodeName, string vatExCode = "", string vatExDesc = "",
            List<LineLevelAllowance> lineLevelAllowances = null, List<LineLevelCharge> lineLevelCharges = null,
            string invoiceLineType = "REGULAR", string prepaidAmount = "0")
        {
            this.LineID = invoiceLineNo;
            this.InvoicedQuantity = quantity;
            this.UnitCode = unitCode;
            this.ItemName = itemName;
            this.TaxCategoryID = taxCategoryID;
            this.TaxPercent = taxPercent;
            this.ItemPrice = itemPrice;
            this.BaseQuantity = baseQuantity;
            this.BaseUnitCode = baseUnitCode;
            this.InvoiceLineType = invoiceLineType;
            this.PrepaidAmount = prepaidAmount;
            this.LineCurrency = priceCurrency;
            this.InvoiceTypeCodename = invoiceTypeCodeName;
            this.VatExCode = vatExCode;
            this.VatExDescription = vatExDesc;

            ReCalculateLine();

            this.LineLevelAllowances = lineLevelAllowances ?? new List<LineLevelAllowance>();
            this.LineLevelCharges = lineLevelCharges ?? new List<LineLevelCharge>();
        }

        public void PopulateInvoiceLine(string invoiceLineNo, string quantity, string unitCode, string itemName,
            string taxCategoryID, string taxPercent, string itemPrice, string priceCurrency, string baseQuantity,
            string baseUnitCode, string vatExCode = "", string vatExDesc = "",
            List<LineLevelAllowance> lineLevelAllowances = null, List<LineLevelCharge> lineLevelCharges = null,
            string invoiceLineType = "REGULAR", string prepaidAmount = "0")
        {
            this.LineID = invoiceLineNo;
            this.InvoicedQuantity = quantity;
            this.UnitCode = unitCode;
            this.ItemName = itemName;
            this.TaxCategoryID = taxCategoryID;
            this.TaxPercent = taxPercent;
            this.ItemPrice = itemPrice;
            this.BaseQuantity = baseQuantity;
            this.BaseUnitCode = baseUnitCode;
            this.InvoiceLineType = invoiceLineType;
            this.PrepaidAmount = prepaidAmount;
            this.LineCurrency = priceCurrency;
            this.VatExCode = vatExCode;
            this.VatExDescription = vatExDesc;

            ReCalculateLine();

            this.LineLevelAllowances = lineLevelAllowances ?? new List<LineLevelAllowance>();
            this.LineLevelCharges = lineLevelCharges ?? new List<LineLevelCharge>();
        }

        public void ReCalculateLine()
        {
            decimal itemPriceDecimal = Convert.ToDecimal(this.ItemPrice);
            decimal invoicedQuantityDecimal = Convert.ToDecimal(this.InvoicedQuantity);
            decimal taxPercentDecimal = Convert.ToDecimal(this.TaxPercent);
            decimal allowanceChargeAmountDecimal = 0;

            if (this.LineLevelAllowances != null)
            {
                foreach (var allowance in this.LineLevelAllowances)
                {
                    allowanceChargeAmountDecimal -= Convert.ToDecimal(allowance.Amount);
                }
            }

            if (this.LineLevelCharges != null)
            {
                foreach (var charge in this.LineLevelCharges)
                {
                    allowanceChargeAmountDecimal += Convert.ToDecimal(charge.Amount);
                }
            }

            decimal itemPriceAmount;
            if (InvoiceLineType == "REGULAR")
            {
                if (this.InvoiceTypeCodename == "0200000")
                {
                    itemPriceAmount = ((itemPriceDecimal * invoicedQuantityDecimal) + allowanceChargeAmountDecimal) * 100 / (taxPercentDecimal + 100) / invoicedQuantityDecimal;
                }
                else
                {
                    itemPriceAmount = itemPriceDecimal;
                }
            }
            else
            {
                itemPriceAmount = 0;
            }

            this.PriceAmount = itemPriceAmount.ToString("F2");

            decimal lineExtensionAmount;
            if (this.InvoiceTypeCodename == "0200000")
            {
                lineExtensionAmount = (itemPriceAmount * invoicedQuantityDecimal) / decimal.Parse(BaseQuantity);
            }
            else
            {
                lineExtensionAmount = ((itemPriceAmount * invoicedQuantityDecimal) / decimal.Parse(BaseQuantity)) + allowanceChargeAmountDecimal;
            }

            lineExtensionAmount = Math.Round(lineExtensionAmount, 2, MidpointRounding.AwayFromZero);
            this.LineExtensionAmount = lineExtensionAmount.ToString("F2");

            decimal taxAmount = lineExtensionAmount * taxPercentDecimal / 100;
            taxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero);
            this.TaxAmount = taxAmount.ToString("F2");

            decimal roundingAmount = lineExtensionAmount + taxAmount;
            roundingAmount = Math.Round(roundingAmount, 2, MidpointRounding.AwayFromZero);
            this.RoundingAmount = roundingAmount.ToString("F2");
            this.AllowanceChargeAmount = allowanceChargeAmountDecimal.ToString("F2");
            this.LineNetAmount = (lineExtensionAmount + taxAmount).ToString("F2");

            if (InvoiceLineType == "PREPAID")
            {
                decimal tempPrepaidAmount = Convert.ToDecimal(this.PrepaidAmount);
                decimal prepaidTaxSubTotalTaxableAmount = Math.Round((tempPrepaidAmount / (taxPercentDecimal + 100)) * 100, 2, MidpointRounding.AwayFromZero);
                decimal prepaidTaxSubTotalTaxAmount = Math.Round(prepaidTaxSubTotalTaxableAmount * taxPercentDecimal / 100, 2, MidpointRounding.AwayFromZero);

                PrepaidTaxSubTotalTaxableAmount = (tempPrepaidAmount - prepaidTaxSubTotalTaxAmount).ToString();
                PrepaidTaxSubTotalTaxAmount = prepaidTaxSubTotalTaxAmount.ToString();
                this.LineNetAmount = (-tempPrepaidAmount).ToString("F2");
            }
        }

        public void AddLineLevelAllowance(string reason, string amount)
        {
            this.LineLevelAllowances.Add(new LineLevelAllowance(reason, amount));
            ReCalculateLine();
        }

        public void AddLineLevelCharge(string reason, string amount)
        {
            this.LineLevelCharges.Add(new LineLevelCharge(reason, amount));
            ReCalculateLine();
        }
    }

    public class UniqeTaxCategory
    {
        public string VatCategoryCode { get; set; }
        public string VatCategoryRate { get; set; }
        public string CatTaxableAmount { get; set; }
        public string CategoryVatAmount { get; set; }
        public string VatExCode { get; set; }
        public string VatExDesc { get; set; }
    }

   
}