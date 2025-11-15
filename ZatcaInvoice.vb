Imports System
'Imports System.Net.Http
'Imports System.Net.Http.Headers
'Imports System.Security.Policy
Imports System.Xml
'Imports com.sun.source.tree
'Imports com.sun.tools.internal.xjc
'Imports java.util
'Imports System.IO
'Imports javax.sound.sampled
'Imports net.sf.saxon.expr.Component
'Imports net.sf.saxon.functions
'Imports Newtonsoft.Json
Imports System.Text
'Imports Org.BouncyCastle.Math.EC

Imports System.Runtime.InteropServices
'Imports com.sun.swing.internal.plaf.metal.resources
'Imports com.sun.xml.internal.ws.api.model
'Imports sun.tools.tree
'Imports ZatcaLibrary.ZatcaInvoice
Imports ZatcaInvoice.ZatcaInvoice


<Guid("1EEC0B55-DD22-4BAA-A2AA-0DCCB961129A")>
Public Class ZatcaInvoice
    Public Property Id As String
    Public Property InvoiceUUID As String
    Public Property InvoiceTypeCodename As String

    Public Property InvoiceCategory As String ' user provided value of code name such as 020000 change into the InvoiceTypeCodename
    Public Property InvoiceTypeCodeValue As String

    Public Property InvoiceType As String ' user provided value such as 388 will update the InvoiceTypeCodeValue
    Public Property ICV As Integer 'UNIQUE INCREMANTAL COUNTER VALUE PER EGS (EINVOICING GENERATION SOLUTION)
    Public Property PIH As String 'MANDATORY FIELD AFTER THE FIRST INVOICE
    Public Property InstructionNote As String
    Public Property IssueDate As String
    Public Property IssueTime As String
    Public Property Currency As String
    Public Property TaxCurrency As String




    'Public VATNumber As String

    Public ActualDeliveryDate As String ' Deliver Date
    Public LatestDeliveryDate As String ' Latest Delivery Date
    Public PaymentMeansCode As String ' Payment Means Code


    ' TaxTotal Node
    Public Property TotalTaxableAmount As String
    Public Property TaxTotalAmount As String
    Public Property TaxCategoryCode As String
    Public Property VATRate As String

    ' LEGAL MONETARY TOTALS
    Public Property LineExtensionAmount As String
    Public Property TaxExclusiveAmount As String
    Public Property TaxInclusiveAmount As String
    Public Property AllowanceTotalAmount As String
    Public Property ChargeTotalAmount As String
    Public Property PayableRoundAmount As String
    Public Property PrepaidAmount As String = "0.00" ' Advance amount
    Public Property PayableAmount As String
    Public Property TotalAmount As String
    Public Property TotalTaxAmount As String


    ' sub classes
    Public Property AccountingSupplier As New Supplier()
    Public Property AccountingCustomer As New Customer()


    ' List of sub classes
    Public Property InvoiceLines As List(Of InvoiceLineItem)
    Public Property DocumentLevelAllowances As List(Of DocumentLevelAllowance)
    Public Property DocumentLevelCharges As List(Of DocumentLevelCharge)


    ' i case of credit note or Debit Note
    Public Property PreviousInvoiceNumber As String
    Public Property CreditDebitNoteReason As String


    ' RETURN A VALUE AND INVOICE CREATION STATUS
    Public Property InvoiceCreationStatusCode As String
    Public Property InvoiceCreationStatus As String

    'Documentlevel tax exception
    Public Property TaxExcemptioReasonCode As String
    Public Property TaxExcemptionReason As String

    Public Property LibraryVersion As String = "1.2.0"
    Public Property LicenseKey As String = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Im1hcmtvbmVnb2xkeSIsIm5hbWVpZCI6IjIyMmNlNjhkLWI4ZTctNDc3OS04M2EwLTZmYjk3ZjljNTYyZiIsImVtYWlsIjoiZGlhbmFndW5hd2FyZGFuYUBnbWFpbC5jb20iLCJUZW5hbnRJZCI6IjQxNjVFRkMzNTkiLCJQcm9kdWN0IjoiUGVycGV0dWFsIiwibmJmIjoxNzU0NTAwNjMwLCJleHAiOjIwNzAwMzM0MzAsImlhdCI6MTc1NDUwMDYzMCwiaXNzIjoiaHR0cHM6Ly93d3cucHJpc3RpbmVpbnZvaWNlLmNvbSIsImF1ZCI6Imh0dHBzOi8vd3d3LnByaXN0aW5laW52b2ljZS5jb20ifQ.TE8TACCSKSqdloaiHghASdgLBVmLHtqc5tMnE00eHE_vHDSR3nX3qX27Dy-WpPcLNle8vkcTsDHfPHXZaCjrIg" 'markone
    'Public Property LicenseKey As String = ""

    Public Property DocumentVatCategories As List(Of UniqeTaxCategory)

    Public Sub New()

    End Sub

    ' Parameterized Constructor
    Public Sub New(invoiceUUID As String,
        id As String, InvoiceCategory As String, InvoiceType As String, icv As Integer, pih As String,
        instructionNote As String, issueDate As String, issueTime As String, currency As String, taxCurrency As String,
        ActualDeliveryDate As String, LatestDeliveryDate As String, paymentMeansCode As String,
        taxCategoryCode As String, vatRate As String,
        Optional accountingSupplier As Supplier = Nothing, Optional accountingCustomer As Customer = Nothing,
        Optional invoiceLines As List(Of InvoiceLineItem) = Nothing,
        Optional documentLevelAllowances As List(Of DocumentLevelAllowance) = Nothing,
        Optional documentLevelCharges As List(Of DocumentLevelCharge) = Nothing, Optional PreviousInvoiceNumber As String = "", Optional CreditDebitNoteReason As String = ""
    )





        Me.Id = id
        Me.InvoiceUUID = invoiceUUID


        If InvoiceCategory = "SIMPLIFIED" Then
            Me.InvoiceTypeCodename = "0200000"
        ElseIf InvoiceCategory = "TAXINVOICE" Then
            Me.InvoiceTypeCodename = "0100000"
        Else
            Me.InvoiceCreationStatusCode = "FAILED"
            Me.InvoiceCreationStatus = "Valid Invoice Category was not provided"
        End If

        If InvoiceType = "INVOICE" Then
            Me.InvoiceTypeCodeValue = "388"
        ElseIf InvoiceType = "CREDITNOTE" Then
            Me.InvoiceTypeCodeValue = "381"

        ElseIf InvoiceType = "DEBITNOTE" Then
            Me.InvoiceTypeCodeValue = "383"
        ElseIf InvoiceType = "PREPAYMENT" Then
            Me.InvoiceTypeCodeValue = "386"
        Else
            Me.InvoiceCreationStatusCode = "FAILED"
            Me.InvoiceCreationStatus = "Valid Invoice Type was not provided"
        End If


        Me.InvoiceCategory = InvoiceCategory
        Me.InvoiceType = InvoiceType
        Me.ICV = icv
        Me.PIH = pih
        Me.InstructionNote = instructionNote
        Me.IssueDate = issueDate
        Me.IssueTime = issueTime
        Me.Currency = currency
        Me.TaxCurrency = taxCurrency

        Me.ActualDeliveryDate = ActualDeliveryDate
        Me.LatestDeliveryDate = LatestDeliveryDate
        Me.PaymentMeansCode = paymentMeansCode

        Me.TaxCategoryCode = taxCategoryCode
        Me.VATRate = vatRate



        Me.PreviousInvoiceNumber = PreviousInvoiceNumber
        Me.CreditDebitNoteReason = CreditDebitNoteReason

        ' Initialize subclasses
        Me.AccountingSupplier = If(accountingSupplier, New Supplier())
        Me.AccountingCustomer = If(accountingCustomer, New Customer())

        ' Initialize lists
        Me.InvoiceLines = If(invoiceLines, New List(Of InvoiceLineItem)())
        Me.DocumentLevelAllowances = If(documentLevelAllowances, New List(Of DocumentLevelAllowance)())
        Me.DocumentLevelCharges = If(documentLevelCharges, New List(Of DocumentLevelCharge)())

        Me.InvoiceCreationStatusCode = "SUCCESS"
    End Sub


    Public Sub PopulateInvoice(invoiceUUID As String,
        id As String, InvoiceCategory As String, InvoiceType As String, icv As Integer, pih As String,
        instructionNote As String, issueDate As String, issueTime As String, currency As String, taxCurrency As String,
        ActualDeliveryDate As String, LatestDeliveryDate As String, paymentMeansCode As String,
        taxCategoryCode As String, vatRate As String, Optional PreviousInvoiceNumber As String = "", Optional CreditDebitNoteReason As String = "",
        Optional accountingSupplier As Supplier = Nothing, Optional accountingCustomer As Customer = Nothing,
        Optional invoiceLines As List(Of InvoiceLineItem) = Nothing,
        Optional documentLevelAllowances As List(Of DocumentLevelAllowance) = Nothing,
        Optional documentLevelCharges As List(Of DocumentLevelCharge) = Nothing)

        Me.Id = id
        Me.InvoiceUUID = invoiceUUID


        If InvoiceCategory = "SIMPLIFIED" Then
            Me.InvoiceTypeCodename = "0200000"
        ElseIf InvoiceCategory = "TAXINVOICE" Then
            Me.InvoiceTypeCodename = "0100000"
        Else
            Me.InvoiceCreationStatusCode = "FAILED"
            Me.InvoiceCreationStatus = "Valid Invoice Category was not provided"
        End If

        If InvoiceType = "INVOICE" Then
            Me.InvoiceTypeCodeValue = "388"
        ElseIf InvoiceType = "CREDITNOTE" Then
            Me.InvoiceTypeCodeValue = "381"

        ElseIf InvoiceType = "DEBITNOTE" Then
            Me.InvoiceTypeCodeValue = "383"
        ElseIf InvoiceType = "PREPAYMENT" Then
            Me.InvoiceTypeCodeValue = "386"
        Else
            Me.InvoiceCreationStatusCode = "FAILED"
            Me.InvoiceCreationStatus = "Valid Invoice Type was not provided"
        End If


        Me.InvoiceCategory = InvoiceCategory
        Me.InvoiceType = InvoiceType
        Me.ICV = icv
        Me.PIH = pih
        Me.InstructionNote = instructionNote
        Me.IssueDate = issueDate
        Me.IssueTime = issueTime
        Me.Currency = currency
        Me.TaxCurrency = taxCurrency

        Me.ActualDeliveryDate = ActualDeliveryDate
        Me.LatestDeliveryDate = LatestDeliveryDate
        Me.PaymentMeansCode = paymentMeansCode

        Me.TaxCategoryCode = taxCategoryCode
        Me.VATRate = vatRate



        Me.PreviousInvoiceNumber = PreviousInvoiceNumber
        Me.CreditDebitNoteReason = CreditDebitNoteReason

        ' Initialize subclasses
        Me.AccountingSupplier = If(accountingSupplier, New Supplier())
        Me.AccountingCustomer = If(accountingCustomer, New Customer())

        ' Initialize lists
        Me.InvoiceLines = If(invoiceLines, New List(Of InvoiceLineItem)())
        Me.DocumentLevelAllowances = If(documentLevelAllowances, New List(Of DocumentLevelAllowance)())
        Me.DocumentLevelCharges = If(documentLevelCharges, New List(Of DocumentLevelCharge)())


        Me.InvoiceCreationStatusCode = "SUCCESS"

    End Sub


    Public Sub SaveXmlInvoice(xmlPath As String)
        Dim updatedInvoice As XmlDocument = Me.GenerateXml()
        updatedInvoice.Save(xmlPath)

    End Sub


    Public Function GenerateXml() As XmlDocument
        Dim newDoc As New XmlDocument()


        Dim xmlDeclaration As XmlDeclaration = newDoc.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
        newDoc.AppendChild(xmlDeclaration)



        ' Define namespaces
        Dim ns As String = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"
        Dim nsCac As String = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"
        Dim nsCbc As String = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"
        Dim nsExt As String = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"

        ' Create root element with namespaces in the correct order
        Dim root As XmlElement = newDoc.CreateElement("Invoice", ns)
        root.SetAttribute("xmlns", ns)
        root.SetAttribute("xmlns:cac", nsCac)
        root.SetAttribute("xmlns:cbc", nsCbc)
        root.SetAttribute("xmlns:ext", nsExt)
        newDoc.AppendChild(root)

        ' Create UUID
        'Dim newGuid As String = Guid.NewGuid().ToString()


        AppendElement(newDoc, root, "cbc:ProfileID", nsCbc, "reporting:1.0")
        ' Add Invoice ID
        AppendElement(newDoc, root, "cbc:ID", nsCbc, Id)

        ' Add UUID
        AppendElement(newDoc, root, "cbc:UUID", nsCbc, InvoiceUUID)

        ' Add Issue Date & Time
        AppendElement(newDoc, root, "cbc:IssueDate", nsCbc, IssueDate)
        AppendElement(newDoc, root, "cbc:IssueTime", nsCbc, IssueTime)

        ' Add Invoice Type Code
        Dim invoiceTypeCodeElement As XmlElement = AppendElement(newDoc, root, "cbc:InvoiceTypeCode", nsCbc, InvoiceTypeCodeValue)
        invoiceTypeCodeElement.SetAttribute("name", InvoiceTypeCodename)

        Dim invoiceNote As XmlElement = AppendElement(newDoc, root, "cbc:Note", nsCbc, InstructionNote)
        invoiceNote.SetAttribute("languageID", "ar")

        ' Add Currency
        AppendElement(newDoc, root, "cbc:DocumentCurrencyCode", nsCbc, Currency)
        AppendElement(newDoc, root, "cbc:TaxCurrencyCode", nsCbc, TaxCurrency)


        If InvoiceTypeCodeValue = "381" Or InvoiceTypeCodeValue = "383" Then
            ' Add BillingReference node
            Dim billingReferenceNode As XmlElement = AppendContainerElement(newDoc, root, "cac:BillingReference", nsCac)
            Dim invoiceDocumentReferenceNode As XmlElement = AppendContainerElement(newDoc, billingReferenceNode, "cac:InvoiceDocumentReference", nsCac)
            AppendElement(newDoc, invoiceDocumentReferenceNode, "cbc:ID", nsCbc, PreviousInvoiceNumber)
        End If



        ' Add Additional Document Reference (ICV)
        Dim icvRef As XmlElement = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac)
        AppendElement(newDoc, icvRef, "cbc:ID", nsCbc, "ICV")
        AppendElement(newDoc, icvRef, "cbc:UUID", nsCbc, ICV.ToString())

        ' Add Additional Document Reference (PIH)
        Dim pihRef As XmlElement = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac)
        AppendElement(newDoc, pihRef, "cbc:ID", nsCbc, "PIH")
        Dim attachment As XmlElement = AppendContainerElement(newDoc, pihRef, "cac:Attachment", nsCac)
        If PIH = "" Then
            PIH = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ=="
        End If
        'AppendElement(newDoc, attachment, "cbc:EmbeddedDocumentBinaryObject", nsCbc, PIH)
        Dim PihBinaryObject As XmlElement = AppendElement(newDoc, attachment, "cbc:EmbeddedDocumentBinaryObject", nsCbc, PIH)
        PihBinaryObject.SetAttribute("mimeCode", "text/plain")



        ' Add QR Code
        Dim additionalDocumentReferenceNode As XmlElement = AppendContainerElement(newDoc, root, "cac:AdditionalDocumentReference", nsCac)
        AppendElement(newDoc, additionalDocumentReferenceNode, "cbc:ID", nsCbc, "QR")
        Dim attachmentNode As XmlElement = AppendContainerElement(newDoc, additionalDocumentReferenceNode, "cac:Attachment", nsCac)
        'Dim embeddedDocumentBinaryObjectNode As XmlElement = AppendElement(newDoc, attachmentNode, "cbc:EmbeddedDocumentBinaryObject", nsCbc, "ARNBY21lIFdpZGdldOKAmXMgTFREAg8zMTExMTExMTExMDExMTMDFDIwMjEtMDEtMDVUMDk6MzI6NDBaBAYyNTAuMDAFBDAuMDAGLG5CNTVSbENQajFhRjFkcC9zUkcrVTI2aGc4UDA2TTlCdFRNeHNlb2N5ckU9B2BNRVlDSVFDNFZvZlJxdlZJUng3VUoxTzl4Vjl3SjVFUTdBZk5UV3BnTFdEWEhpSlZZd0loQUw1S040TDR6WUhrMnVwWjlOWmUxYjJwK0pldWtTcEM2OXMyZ284ZXNBaVoIWDBWMBAGByqGSM49AgEGBSuBBAAKA0IABGGDDKDmhWAITDv7LXqLX2cmr6+qddUkpcLCvWs5rC2O29W/hS4ajAK4Qdnahym6MaijX75Cg3j4aao7ouYXJ9EJRjBEAiA6CM08lbTXuWwiKOZVBWQ/sbMU7YpAp30Ydq6QuAhYWwIgUkX27AqFMzEONZs37VrCycUjtEsFHED/qFn4XXC1qpQ=")

        Dim qrBinaryObject As XmlElement = AppendElement(newDoc, attachmentNode, "cbc:EmbeddedDocumentBinaryObject", nsCbc, "ARNBY21lIFdpZGdldOKAmXMgTFREAg8zMTExMTExMTExMDExMTMDFDIwMjEtMDEtMDVUMDk6MzI6NDBaBAYyNTAuMDAFBDAuMDAGLG5CNTVSbENQajFhRjFkcC9zUkcrVTI2aGc4UDA2TTlCdFRNeHNlb2N5ckU9B2BNRVlDSVFDNFZvZlJxdlZJUng3VUoxTzl4Vjl3SjVFUTdBZk5UV3BnTFdEWEhpSlZZd0loQUw1S040TDR6WUhrMnVwWjlOWmUxYjJwK0pldWtTcEM2OXMyZ284ZXNBaVoIWDBWMBAGByqGSM49AgEGBSuBBAAKA0IABGGDDKDmhWAITDv7LXqLX2cmr6+qddUkpcLCvWs5rC2O29W/hS4ajAK4Qdnahym6MaijX75Cg3j4aao7ouYXJ9EJRjBEAiA6CM08lbTXuWwiKOZVBWQ/sbMU7YpAp30Ydq6QuAhYWwIgUkX27AqFMzEONZs37VrCycUjtEsFHED/qFn4XXC1qpQ=")
        qrBinaryObject.SetAttribute("mimeCode", "text/plain")



        ' ADD SIGNATURE BLOCK
        Dim signatureNode As XmlElement = AppendContainerElement(newDoc, root, "cac:Signature", nsCac)
        AppendElement(newDoc, signatureNode, "cbc:ID", nsCbc, "urn:oasis:names:specification:ubl:signature:Invoice")
        AppendElement(newDoc, signatureNode, "cbc:SignatureMethod", nsCbc, "urn:oasis:names:specification:ubl:dsig:enveloped:xades")


        ' Add Supplier Information
        Dim supplierNode As XmlElement = AppendContainerElement(newDoc, root, "cac:AccountingSupplierParty", nsCac)
        Dim supplierParty As XmlElement = AppendContainerElement(newDoc, supplierNode, "cac:Party", nsCac)
        Dim partyIdentification As XmlElement = AppendContainerElement(newDoc, supplierParty, "cac:PartyIdentification", nsCac)
        '<cbc:ID schemeID="CRN">133333333314444</cbc:ID>
        'Dim PartyId As XmlElement = AppendContainerElement(newDoc, partyIdentification, "cbc:ID", nsCbc)
        AppendElement(newDoc, partyIdentification, "cbc:ID", nsCbc, AccountingSupplier.IdNo).SetAttribute("schemeID", AccountingSupplier.IdType)
        'AppendElement(newDoc, PartyId, "schemeID", nsCbc, "CRN")


        Dim postalAddress As XmlElement = AppendContainerElement(newDoc, supplierParty, "cac:PostalAddress", nsCac)
        AppendElement(newDoc, postalAddress, "cbc:StreetName", nsCbc, AccountingSupplier.StreetName)
        AppendElement(newDoc, postalAddress, "cbc:BuildingNumber", nsCbc, AccountingSupplier.BuildingNo)
        AppendElement(newDoc, postalAddress, "cbc:PlotIdentification", nsCbc, AccountingSupplier.PlotNumber)
        AppendElement(newDoc, postalAddress, "cbc:CitySubdivisionName", nsCbc, AccountingSupplier.CitySubdivision)
        AppendElement(newDoc, postalAddress, "cbc:CityName", nsCbc, AccountingSupplier.City)
        AppendElement(newDoc, postalAddress, "cbc:PostalZone", nsCbc, AccountingSupplier.PostalCode)

        Dim country As XmlElement = AppendContainerElement(newDoc, postalAddress, "cac:Country", nsCac)
        AppendElement(newDoc, country, "cbc:IdentificationCode", nsCbc, "SA") ' Saudi Arabia code

        Dim partyTaxScheme As XmlElement = AppendContainerElement(newDoc, supplierParty, "cac:PartyTaxScheme", nsCac)
        AppendElement(newDoc, partyTaxScheme, "cbc:CompanyID", nsCbc, AccountingSupplier.VATNumber)

        Dim taxScheme As XmlElement = AppendContainerElement(newDoc, partyTaxScheme, "cac:TaxScheme", nsCac)
        AppendElement(newDoc, taxScheme, "cbc:ID", nsCbc, "VAT")

        Dim partyLegalEntity As XmlElement = AppendContainerElement(newDoc, supplierParty, "cac:PartyLegalEntity", nsCac)
        AppendElement(newDoc, partyLegalEntity, "cbc:RegistrationName", nsCbc, AccountingSupplier.CompanName)


        ' Add Customer Information
        Dim customerNode As XmlElement = AppendContainerElement(newDoc, root, "cac:AccountingCustomerParty", nsCac)
        Dim customerParty As XmlElement = AppendContainerElement(newDoc, customerNode, "cac:Party", nsCac)

        Dim CUSpartyIdentification As XmlElement = AppendContainerElement(newDoc, customerParty, "cac:PartyIdentification", nsCac)
        AppendElement(newDoc, CUSpartyIdentification, "cbc:ID", nsCbc, AccountingCustomer.BuyerIdNumber).SetAttribute("schemeID", AccountingCustomer.BuyerIdType)

        Dim cusPostalAddress As XmlElement = AppendContainerElement(newDoc, customerParty, "cac:PostalAddress", nsCac)
        AppendElement(newDoc, cusPostalAddress, "cbc:StreetName", nsCbc, AccountingCustomer.Street)
        AppendElement(newDoc, cusPostalAddress, "cbc:BuildingNumber", nsCbc, AccountingCustomer.BuildingNo)
        AppendElement(newDoc, cusPostalAddress, "cbc:CitySubdivisionName", nsCbc, AccountingCustomer.CitySubdivision)
        AppendElement(newDoc, cusPostalAddress, "cbc:CityName", nsCbc, AccountingCustomer.City)
        AppendElement(newDoc, cusPostalAddress, "cbc:PostalZone", nsCbc, AccountingCustomer.PostalCode)

        Dim cusCountry As XmlElement = AppendContainerElement(newDoc, cusPostalAddress, "cac:Country", nsCac)
        AppendElement(newDoc, cusCountry, "cbc:IdentificationCode", nsCbc, "SA") ' Saudi Arabia code


        If InvoiceTypeCodename = "0100000" Then
            Dim cuspartyTaxScheme As XmlElement = AppendContainerElement(newDoc, customerParty, "cac:PartyTaxScheme", nsCac)

            If AccountingCustomer.VATNumber <> "" Then ' if standard invoice
                AppendElement(newDoc, cuspartyTaxScheme, "cbc:CompanyID", nsCbc, AccountingCustomer.VATNumber)
            End If

            Dim custaxScheme As XmlElement = AppendContainerElement(newDoc, cuspartyTaxScheme, "cac:TaxScheme", nsCac)
            AppendElement(newDoc, custaxScheme, "cbc:ID", nsCbc, "VAT")
        End If







        Dim cuspartyLegalEntity As XmlElement = AppendContainerElement(newDoc, customerParty, "cac:PartyLegalEntity", nsCac)
        AppendElement(newDoc, cuspartyLegalEntity, "cbc:RegistrationName", nsCbc, AccountingCustomer.PartyName)





        'DELIVERY INFORMATION
        Dim deliveryNode As XmlElement = AppendContainerElement(newDoc, root, "cac:Delivery", nsCac)
        AppendElement(newDoc, deliveryNode, "cbc:ActualDeliveryDate", nsCbc, ActualDeliveryDate)
        AppendElement(newDoc, deliveryNode, "cbc:LatestDeliveryDate", nsCbc, LatestDeliveryDate)


        ' Payment Means
        Dim paymentMeansNode As XmlElement = AppendContainerElement(newDoc, root, "cac:PaymentMeans", nsCac)
        AppendElement(newDoc, paymentMeansNode, "cbc:PaymentMeansCode", nsCbc, PaymentMeansCode)

        If InvoiceTypeCodeValue = "381" Or InvoiceTypeCodeValue = "383" Then
            AppendElement(newDoc, paymentMeansNode, "cbc:InstructionNote", nsCbc, CreditDebitNoteReason)
        End If

        ' ** Add Allowance (Repeating Node Based on List)**
        If DocumentLevelAllowances IsNot Nothing Then
            For Each allowance As DocumentLevelAllowance In DocumentLevelAllowances

                Dim allowanceChargeNode As XmlElement = AppendContainerElement(newDoc, root, "cac:AllowanceCharge", nsCac)

                ' Add ChargeIndicator element (using the actual Boolean value)
                AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "false")

                ' Add AllowanceChargeReason element
                AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, allowance.AllowanceChargeReason)
                'AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReasonCode", nsCbc, allowance.AllowanceChargeReasonCode)

                ' Add Amount element (convert Decimal to String) and set currencyID attribute
                AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, allowance.Amount.ToString()).SetAttribute("currencyID", "SAR")

                ' Add Tax Category under AllowanceCharge
                Dim taxCategoryNode As XmlElement = AppendContainerElement(newDoc, allowanceChargeNode, "cac:TaxCategory", nsCac)

                ' Add ID element inside TaxCategory and set attributes manually
                Dim taxCategoryIdElement1 As XmlElement = newDoc.CreateElement("cbc:ID", nsCbc)
                taxCategoryIdElement1.InnerText = allowance.TaxCategoryCode
                taxCategoryIdElement1.SetAttribute("schemeID", "UN/ECE 5305")
                taxCategoryIdElement1.SetAttribute("schemeAgencyID", "6")
                taxCategoryNode.AppendChild(taxCategoryIdElement1)

                ' Add Percent element inside TaxCategory
                AppendElement(newDoc, taxCategoryNode, "cbc:Percent", nsCbc, allowance.TaxPercent.ToString())

                ' Add TaxScheme under TaxCategory
                Dim taxSchemeNode As XmlElement = AppendContainerElement(newDoc, taxCategoryNode, "cac:TaxScheme", nsCac)

                ' Add ID element inside TaxScheme and set attributes manually
                Dim taxSchemeIdElement As XmlElement = newDoc.CreateElement("cbc:ID", nsCbc)
                taxSchemeIdElement.InnerText = "VAT"
                taxSchemeIdElement.SetAttribute("schemeID", "UN/ECE 5153")
                taxSchemeIdElement.SetAttribute("schemeAgencyID", "6")
                taxSchemeNode.AppendChild(taxSchemeIdElement)

            Next
        End If

        ' ** Add Charges (Repeating Node Based on List)**
        If DocumentLevelCharges IsNot Nothing Then
            For Each charges As DocumentLevelCharge In DocumentLevelCharges

                Dim allowanceChargeNode As XmlElement = AppendContainerElement(newDoc, root, "cac:AllowanceCharge", nsCac)

                ' Add ChargeIndicator element (using the actual Boolean value)
                AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true")

                ' Add AllowanceChargeReason element
                AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReasonCode", nsCbc, "CG")
                AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, charges.AllowanceChargeReason)


                ' Add Amount element (convert Decimal to String) and set currencyID attribute
                AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, charges.Amount.ToString()).SetAttribute("currencyID", "SAR")

                ' Add Tax Category under AllowanceCharge
                Dim taxCategoryNode As XmlElement = AppendContainerElement(newDoc, allowanceChargeNode, "cac:TaxCategory", nsCac)


                Dim taxCategoryIdElement2 As XmlElement = newDoc.CreateElement("cbc:ID", nsCbc)
                taxCategoryIdElement2.InnerText = charges.TaxCategoryCode
                taxCategoryIdElement2.SetAttribute("schemeID", "UN/ECE 5305")
                taxCategoryIdElement2.SetAttribute("schemeAgencyID", "6")
                taxCategoryNode.AppendChild(taxCategoryIdElement2)

                ' Add Percent element inside TaxCategory
                AppendElement(newDoc, taxCategoryNode, "cbc:Percent", nsCbc, charges.TaxPercent.ToString())

                ' Add TaxScheme under TaxCategory
                Dim taxSchemeNode As XmlElement = AppendContainerElement(newDoc, taxCategoryNode, "cac:TaxScheme", nsCac)

                ' Add ID element inside TaxScheme and set attributes manually
                Dim taxSchemeIdElement As XmlElement = newDoc.CreateElement("cbc:ID", nsCbc)
                taxSchemeIdElement.InnerText = "VAT"
                taxSchemeIdElement.SetAttribute("schemeID", "UN/ECE 5153")
                taxSchemeIdElement.SetAttribute("schemeAgencyID", "6")
                taxSchemeNode.AppendChild(taxSchemeIdElement)

            Next
        End If


        ' TAX TOTAL NODE
        Dim taxTotalNode1 As XmlElement = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac)
        AppendElement(newDoc, taxTotalNode1, "cbc:TaxAmount", nsCbc, TotalTaxAmount).SetAttribute("currencyID", "SAR")



        ' 2nd tax total nodes with sub totals
        Dim taxTotalNode As XmlElement = AppendContainerElement(newDoc, root, "cac:TaxTotal", nsCac)
        AppendElement(newDoc, taxTotalNode, "cbc:TaxAmount", nsCbc, TotalTaxAmount).SetAttribute("currencyID", "SAR")


        ' document tax sub total nodes
        For Each taxcat In DocumentVatCategories

            Dim taxSubtotalNode As XmlElement = AppendContainerElement(newDoc, taxTotalNode, "cac:TaxSubtotal", nsCac)
            'AppendElement(newDoc, taxSubtotalNode, "cbc:TaxableAmount", nsCbc, TotalTaxableAmount).SetAttribute("currencyID", "SAR")
            AppendElement(newDoc, taxSubtotalNode, "cbc:TaxableAmount", nsCbc, taxcat.CatTaxableAmount).SetAttribute("currencyID", "SAR")

            ' sub total tax amount is 0 if document taxcode is E or Z
            If taxcat.VatCategoryCode <> "S" Then
                AppendElement(newDoc, taxSubtotalNode, "cbc:TaxAmount", nsCbc, "0.00").SetAttribute("currencyID", "SAR")
            Else
                AppendElement(newDoc, taxSubtotalNode, "cbc:TaxAmount", nsCbc, taxcat.CategoryVatAmount).SetAttribute("currencyID", "SAR")
            End If

            Dim taxCategoryInSubtotalNode As XmlElement = AppendContainerElement(newDoc, taxSubtotalNode, "cac:TaxCategory", nsCac)

            Dim taxCategoryIdElement As XmlElement = newDoc.CreateElement("cbc:ID", nsCbc)
            taxCategoryIdElement.InnerText = taxcat.VatCategoryCode
            taxCategoryIdElement.SetAttribute("schemeID", "UN/ECE 5305")
            taxCategoryIdElement.SetAttribute("schemeAgencyID", "6")
            taxCategoryInSubtotalNode.AppendChild(taxCategoryIdElement)

            If taxcat.VatCategoryCode <> "S" Then
                AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:Percent", nsCbc, "0.00")
            Else
                'ZTC 6/21
                'AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:Percent", nsCbc, VATRate)
                AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:Percent", nsCbc, taxcat.VatCategoryRate)
            End If


            'ZTC 6/21
            'If taxcat.VatCategoryCode = "Z"
            If taxcat.VatCategoryCode = "Z" Or taxcat.VatCategoryCode = "E" Then
                TaxExcemptioReasonCode = taxcat.VatExCode
                TaxExcemptionReason = taxcat.VatExDesc
                AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:TaxExemptionReasonCode", nsCbc, TaxExcemptioReasonCode)
                AppendElement(newDoc, taxCategoryInSubtotalNode, "cbc:TaxExemptionReason", nsCbc, TaxExcemptionReason)
            End If

            Dim taxSchemeInSubtotalNode As XmlElement = AppendContainerElement(newDoc, taxCategoryInSubtotalNode, "cac:TaxScheme", nsCac)
            Dim element = AppendElement(newDoc, taxSchemeInSubtotalNode, "cbc:ID", nsCbc, "VAT")
            element.SetAttribute("schemeID", "UN/ECE 5153")
            element.SetAttribute("schemeAgencyID", "6")

        Next










        ' ADD LEGAL MONETARY TOTAL
        Dim legalMonetaryTotalNode As XmlElement = AppendContainerElement(newDoc, root, "cac:LegalMonetaryTotal", nsCac)

        AppendElement(newDoc, legalMonetaryTotalNode, "cbc:LineExtensionAmount", nsCbc, LineExtensionAmount).SetAttribute("currencyID", "SAR")

        AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxExclusiveAmount", nsCbc, TaxExclusiveAmount).SetAttribute("currencyID", "SAR")

        AppendElement(newDoc, legalMonetaryTotalNode, "cbc:TaxInclusiveAmount", nsCbc, TaxInclusiveAmount).SetAttribute("currencyID", "SAR")

        If AllowanceTotalAmount > 0 Then
            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:AllowanceTotalAmount", nsCbc, AllowanceTotalAmount).SetAttribute("currencyID", "SAR")
        End If

        If ChargeTotalAmount > 0 Then
            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:ChargeTotalAmount", nsCbc, ChargeTotalAmount).SetAttribute("currencyID", "SAR")
        End If

        If PrepaidAmount > 0 Then
            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PrepaidAmount", nsCbc, PrepaidAmount).SetAttribute("currencyID", "SAR")
        End If

        If PayableRoundAmount <> 0 Then
            AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PayableRoundingAmount", nsCbc, PayableRoundAmount).SetAttribute("currencyID", "SAR")
        End If

        AppendElement(newDoc, legalMonetaryTotalNode, "cbc:PayableAmount", nsCbc, PayableAmount).SetAttribute("currencyID", "SAR")


        ' add invoice lines
        If InvoiceLines IsNot Nothing Then
            For Each invoiceLine As InvoiceLineItem In InvoiceLines

                Dim invoiceLineNode As XmlElement = AppendContainerElement(newDoc, root, "cac:InvoiceLine", nsCac)
                AppendElement(newDoc, invoiceLineNode, "cbc:ID", nsCbc, invoiceLine.LineID)

                Dim invoicedQuantityNode As XmlElement = AppendElement(newDoc, invoiceLineNode, "cbc:InvoicedQuantity", nsCbc, invoiceLine.InvoicedQuantity)
                invoicedQuantityNode.SetAttribute("unitCode", invoiceLine.UnitCode)

                AppendElement(newDoc, invoiceLineNode, "cbc:LineExtensionAmount", nsCbc, invoiceLine.LineExtensionAmount).SetAttribute("currencyID", invoiceLine.LineCurrency)

                'ZTC 6/21
                ' printing line level allowance charge for the standard invoice
                If InvoiceTypeCodename = "0100000" Then
                    ' add line level allowances
                    If invoiceLine.LineLevelAllowances IsNot Nothing Then
                        For Each allowance As LineLevelAllowance In invoiceLine.LineLevelAllowances
                            ' Add AllowanceCharge block under Price
                            Dim allowanceChargeNode1 As XmlElement = AppendContainerElement(newDoc, invoiceLineNode, "cac:AllowanceCharge", nsCac)
                            AppendElement(newDoc, allowanceChargeNode1, "cbc:ChargeIndicator", nsCbc, "false")
                            AppendElement(newDoc, allowanceChargeNode1, "cbc:AllowanceChargeReason", nsCbc, allowance.Reason)
                            AppendElement(newDoc, allowanceChargeNode1, "cbc:Amount", nsCbc, allowance.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency)
                        Next
                    End If

                    'add line level charges
                    If invoiceLine.LineLevelCharges IsNot Nothing Then
                        For Each Charge As LineLevelCharge In invoiceLine.LineLevelCharges
                            ' Add AllowanceCharge block under Price
                            Dim allowanceChargeNode As XmlElement = AppendContainerElement(newDoc, invoiceLineNode, "cac:AllowanceCharge", nsCac)
                            AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true")
                            AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, Charge.Reason)
                            AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, Charge.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency)
                        Next
                    End If
                End If



                ' if prepaid line add prepaid doc reference

                If invoiceLine.InvoiceLineType = "PREPAID" Then
                    Dim PrepaidDocumentReferenceNode As XmlElement = AppendContainerElement(newDoc, invoiceLineNode, "cac:DocumentReference", nsCac)
                    ' Add Invoice ID
                    AppendElement(newDoc, PrepaidDocumentReferenceNode, "cbc:ID", nsCbc, invoiceLine.PrepaidInvoiceId)

                    ' Add UUID
                    'AppendElement(newDoc, invoiceLineNode, "cbc:UUID", nsCbc, InvoiceUUID)

                    ' Add Issue Date & Time
                    AppendElement(newDoc, PrepaidDocumentReferenceNode, "cbc:IssueDate", nsCbc, invoiceLine.PrepaidIssueDate)
                    AppendElement(newDoc, PrepaidDocumentReferenceNode, "cbc:IssueTime", nsCbc, invoiceLine.PrepaidIssueTime)

                    AppendElement(newDoc, PrepaidDocumentReferenceNode, "cbc:DocumentTypeCode", nsCbc, invoiceLine.PrepaidDocumentCode)
                End If


                ' Add TaxTotal block
                Dim linetaxTotalNode As XmlElement = AppendContainerElement(newDoc, invoiceLineNode, "cac:TaxTotal", nsCac)
                AppendElement(newDoc, linetaxTotalNode, "cbc:TaxAmount", nsCbc, invoiceLine.TaxAmount).SetAttribute("currencyID", "SAR")
                AppendElement(newDoc, linetaxTotalNode, "cbc:RoundingAmount", nsCbc, invoiceLine.RoundingAmount).SetAttribute("currencyID", "SAR")


                ' if prepaid line tax sub total
                If invoiceLine.InvoiceLineType = "PREPAID" Then
                    Dim PrepaidTaxSubtotalNode As XmlElement = AppendContainerElement(newDoc, linetaxTotalNode, "cac:TaxSubtotal", nsCac)

                    AppendElement(newDoc, PrepaidTaxSubtotalNode, "cbc:TaxableAmount", nsCbc, invoiceLine.PrepaidTaxSubTotalTaxableAmount).SetAttribute("currencyID", "SAR")
                    AppendElement(newDoc, PrepaidTaxSubtotalNode, "cbc:TaxAmount", nsCbc, invoiceLine.PrepaidTaxSubTotalTaxAmount).SetAttribute("currencyID", "SAR")

                    Dim PrepaidtaxCategoryInSubtotalNode As XmlElement = AppendContainerElement(newDoc, PrepaidTaxSubtotalNode, "cac:TaxCategory", nsCac)

                    Dim PrepaidtaxCategoryIdElement As XmlElement = newDoc.CreateElement("cbc:ID", nsCbc)
                    'comment 7/8
                    'PrepaidtaxCategoryIdElement.InnerText = TaxCategoryCode
                    PrepaidtaxCategoryIdElement.InnerText = invoiceLine.TaxCategoryID

                    'PrepaidtaxCategoryIdElement.SetAttribute("schemeID", "UN/ECE 5305")
                    'PrepaidtaxCategoryIdElement.SetAttribute("schemeAgencyID", "6")
                    PrepaidtaxCategoryInSubtotalNode.AppendChild(PrepaidtaxCategoryIdElement)

                    AppendElement(newDoc, PrepaidtaxCategoryInSubtotalNode, "cbc:Percent", nsCbc, invoiceLine.TaxPercent)

                    Dim PrepaidtaxSchemeInSubtotalNode As XmlElement = AppendContainerElement(newDoc, PrepaidtaxCategoryInSubtotalNode, "cac:TaxScheme", nsCac)

                    Dim Prepaidelement = AppendElement(newDoc, PrepaidtaxSchemeInSubtotalNode, "cbc:ID", nsCbc, "VAT")
                    'Prepaidelement.SetAttribute("schemeID", "UN/ECE 5153")
                    'Prepaidelement.SetAttribute("schemeAgencyID", "6")

                End If



                ' Add Item block
                Dim itemNode As XmlElement = AppendContainerElement(newDoc, invoiceLineNode, "cac:Item", nsCac)
                AppendElement(newDoc, itemNode, "cbc:Name", nsCbc, invoiceLine.ItemName)

                Dim classifiedTaxCategoryNode As XmlElement = AppendContainerElement(newDoc, itemNode, "cac:ClassifiedTaxCategory", nsCac)
                AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:ID", nsCbc, invoiceLine.TaxCategoryID)
                AppendElement(newDoc, classifiedTaxCategoryNode, "cbc:Percent", nsCbc, invoiceLine.TaxPercent)

                Dim taxSchemeNode As XmlElement = AppendContainerElement(newDoc, classifiedTaxCategoryNode, "cac:TaxScheme", nsCac)
                AppendElement(newDoc, taxSchemeNode, "cbc:ID", nsCbc, "VAT")

                ' Add Price block
                Dim priceNode As XmlElement = AppendContainerElement(newDoc, invoiceLineNode, "cac:Price", nsCac)
                AppendElement(newDoc, priceNode, "cbc:PriceAmount", nsCbc, invoiceLine.PriceAmount).SetAttribute("currencyID", invoiceLine.LineCurrency)

                If invoiceLine.InvoiceLineType <> "PREPAID" Then
                    AppendElement(newDoc, priceNode, "cbc:BaseQuantity", nsCbc, invoiceLine.BaseQuantity).SetAttribute("unitCode", invoiceLine.BaseUnitCode)
                End If

                'ZTC 6/21
                'If simplified  then print allowance charges in the price node
                If InvoiceTypeCodename = "0200000" Then
                    ' add line level allowances
                    If invoiceLine.LineLevelAllowances IsNot Nothing Then
                        For Each allowance As LineLevelAllowance In invoiceLine.LineLevelAllowances
                            ' Add AllowanceCharge block under Price
                            Dim allowanceChargeNode1 As XmlElement = AppendContainerElement(newDoc, priceNode, "cac:AllowanceCharge", nsCac)
                            AppendElement(newDoc, allowanceChargeNode1, "cbc:ChargeIndicator", nsCbc, "false")
                            AppendElement(newDoc, allowanceChargeNode1, "cbc:AllowanceChargeReason", nsCbc, allowance.Reason)
                            AppendElement(newDoc, allowanceChargeNode1, "cbc:Amount", nsCbc, allowance.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency)
                        Next
                    End If

                    'add line level charges
                    If invoiceLine.LineLevelCharges IsNot Nothing Then
                        For Each Charge As LineLevelCharge In invoiceLine.LineLevelCharges
                            ' Add AllowanceCharge block under Price
                            Dim allowanceChargeNode As XmlElement = AppendContainerElement(newDoc, priceNode, "cac:AllowanceCharge", nsCac)
                            AppendElement(newDoc, allowanceChargeNode, "cbc:ChargeIndicator", nsCbc, "true")
                            AppendElement(newDoc, allowanceChargeNode, "cbc:AllowanceChargeReason", nsCbc, Charge.Reason)
                            AppendElement(newDoc, allowanceChargeNode, "cbc:Amount", nsCbc, Charge.Amount).SetAttribute("currencyID", invoiceLine.LineCurrency)
                        Next
                    End If
                End If


            Next
        End If

        Return newDoc
    End Function

    ' Helper function to append an element with text content
    Private Function AppendElement(doc As XmlDocument, parent As XmlElement, name As String, ns As String, value As String) As XmlElement
        Dim element As XmlElement = doc.CreateElement(name, ns)
        element.InnerText = value
        parent.AppendChild(element)
        Return element
    End Function

    ' Helper function to append a container element (without text content)
    Private Function AppendContainerElement(doc As XmlDocument, parent As XmlElement, name As String, ns As String) As XmlElement
        Dim element As XmlElement = doc.CreateElement(name, ns)
        parent.AppendChild(element)
        Return element
    End Function

    Public Sub AppendAttribute(newDoc As XmlDocument, parentNode As XmlElement, attrName As String, attrValue As String)
        Dim attribute As XmlAttribute = newDoc.CreateAttribute(attrName)
        attribute.Value = attrValue
        parentNode.Attributes.Append(attribute)
    End Sub


    'Add invoiceLine into the Invoice object by parameters
    Public Sub AddInvoiceLineItem(InvoiceLineNo As String, quantity As String, UnitCode As String,
                  itemName As String, taxCategoryID As String, taxPercent As String,
                  itemPrice As String, pricecurrency As String, BaseQuantity As String, BaseUnitCode As String, Optional Vatexcode As String = "", Optional Vatexdesc As String = "", Optional lineLevelAllowances As List(Of LineLevelAllowance) = Nothing,
                   Optional lineLevelCharges As List(Of LineLevelCharge) = Nothing, Optional invoiceLineType As String = "REGULAR", Optional prepaidAmount As String = "0")

        ' check license
        If LicenseKey = "" Then
            Throw New InvalidOperationException("License key is not supplied")
        End If

        Dim InvalidLicense As Boolean

        Dim currentTime As DateTime = DateTime.Now
        If (currentTime.Hour = 9 OrElse currentTime.Hour = 12 OrElse currentTime.Hour = 16) AndAlso
                    currentTime.Minute >= 0 AndAlso currentTime.Minute <= 29 Then

            Dim licenseService As New LicenseService(LicenseKey)
            Try
                If licenseService.CheckLicenseValidity().Result = False Then
                    ' Do not perform but return
                    InvalidLicense = True


                End If
            Catch ex As Exception
                'allow to proceed silently
            End Try
        End If

        If InvalidLicense Then
            Throw New InvalidOperationException("License is not valid. Operation cannot proceed.")
        End If



        ' Create a new InvoiceLineItem object
        Dim invoiceLine As New InvoiceLineItem(InvoiceLineNo, quantity, UnitCode, itemName, taxCategoryID, taxPercent, itemPrice, pricecurrency, BaseQuantity, BaseUnitCode, InvoiceTypeCodename, Vatexcode, Vatexdesc)


        ' Add the new InvoiceLineItem object to the InvoiceLines list
        InvoiceLines.Add(invoiceLine)
        RecalculateDocument()
    End Sub


    ' add line level charges into invoice lines
    Public Sub AddLineLevelCharges(line As String, Reason As String, Amount As String)
        Dim foundInvoiceLine As InvoiceLineItem = InvoiceLines.Find(Function(item) item.LineID = line)

        foundInvoiceLine.AddLineLevelCharge(Reason, Amount)
        RecalculateDocument()
    End Sub


    ' add line level allowance into invoice line
    Public Sub AddLineLevelAllowance(line As String, Reason As String, Amount As String)

        Dim foundInvoiceLine As InvoiceLineItem = InvoiceLines.Find(Function(item) item.LineID = line)

        foundInvoiceLine.AddLineLevelAllowance(Reason, Amount)
        RecalculateDocument()
    End Sub


    ' expose invoice lines into datatable
    Public Function GetInvoiceLines() As DataTable
        ' Create and populate the DataTable with invoice lines
        Dim dt As New DataTable()
        dt.Columns.Add("LineID", GetType(String))
        dt.Columns.Add("LineExtensionAmt", GetType(String))
        dt.Columns.Add("LineTaxAmt", GetType(String))
        dt.Columns.Add("LineNetAmt", GetType(String))
        dt.Columns.Add("LineAllowanceCharge", GetType(String))

        ' Add data to the DataTable (example)
        For Each line In InvoiceLines
            dt.Rows.Add(line.LineID, line.LineExtensionAmount, line.TaxAmount, line.LineNetAmount, line.AllowanceChargeAmount)
        Next

        Return dt
    End Function

    'expose invoice lins into csv
    Public Function GetInvoiceLinesAsCSV() As String
        Dim dt As DataTable = GetInvoiceLines()
        Dim csv As New StringBuilder()

        ' Write the column headers
        For Each column As DataColumn In dt.Columns
            csv.Append(column.ColumnName & ",")
        Next
        csv.AppendLine()

        ' Write the data rows
        For Each row As DataRow In dt.Rows
            For Each col As DataColumn In dt.Columns
                csv.Append(row(col).ToString() & ",")
            Next
            csv.AppendLine()
        Next

        Return csv.ToString()
    End Function


    Public Sub AddCustomer(partyName As String,
            vatnumber As String,
            idType As String,
            idNumber As String,
            BuildingNo As String,
            street As String,
            citySubdivision As String,
            city As String,
            postalCode As String)

        Dim customer = New Customer(partyName, vatnumber, idType, idNumber, BuildingNo, street, citySubdivision, city, postalCode)

        Me.AccountingCustomer = customer

    End Sub


    Public Sub AddSupplier(idtype As String,
            idno As String,
            companName As String,
            vatnumber As String,
            streetName As String,
            buildingNo As String,
            plotNumber As String,
            citySubdivision As String,
            city As String,
            postalCode As String)

        Dim supplier As New Supplier(idtype, idno, companName, vatnumber, streetName, buildingNo, plotNumber, citySubdivision, city, postalCode)
        Me.AccountingSupplier = supplier


    End Sub


    'Add invoiceLine to the Invoice by lineitem object
    Public Sub AddInvoiceLineItem(ByRef invoiceLine As InvoiceLineItem)


        ' check license
        If LicenseKey = "" Then
            Throw New InvalidOperationException("License key is not supplied")
        End If

        Dim InvalidLicense As Boolean

        Dim currentTime As DateTime = DateTime.Now
        If (currentTime.Hour = 9 OrElse currentTime.Hour = 12 OrElse currentTime.Hour = 16) AndAlso
                    currentTime.Minute >= 0 AndAlso currentTime.Minute <= 29 Then

            Dim licenseService As New LicenseService(LicenseKey)
            Try
                If licenseService.CheckLicenseValidity().Result = False Then
                    ' Do not perform but return
                    InvalidLicense = True


                End If
            Catch ex As Exception
                'allow to proceed silently
            End Try
        End If

        If InvalidLicense Then
            Throw New InvalidOperationException("License is not valid. Operation cannot proceed.")
        End If



        ' Add the new InvoiceLineItem object to the InvoiceLines list
        invoiceLine.InvoiceTypeCodename = Me.InvoiceTypeCodename
        invoiceLine.ReCalculateLine()



        InvoiceLines.Add(invoiceLine)
        RecalculateDocument()
    End Sub


    'Add prepayment line , should called after all invoice lines are added
    Public Sub AddPrepaymentAmount(prepaidAmount As String, quantity As String, UnitCode As String, itemName As String, taxCategoryID As String, taxPercent As String,
                 priceAmount As String, prepayCurrency As String, BaseQuantity As String, BaseUnitCode As String, PrepaidInvoiceId As String, PrepaidInvoiceIssueDate As String, PrepaidInvoiceIssueTime As String, Optional lineLevelAllowances As List(Of LineLevelAllowance) = Nothing,
                   Optional lineLevelCharges As List(Of LineLevelCharge) = Nothing)

        Dim tempPrepaid As Decimal = 0
        'calculate total prepayment amount in the invoice
        For Each line In InvoiceLines
            If line.InvoiceLineType = "PREPAID" Then
                tempPrepaid = tempPrepaid + line.PrepaidAmount
            End If
        Next

        'document level prepaid amount update
        Me.PrepaidAmount = tempPrepaid + prepaidAmount

        ' get last line no
        Dim NewInvoiceLine As String = (InvoiceLines.Count + 1).ToString

        ' Create a new InvoiceLineItem object
        Dim invoiceLine As New InvoiceLineItem(NewInvoiceLine, quantity, UnitCode, "Prepayment adjustment", taxCategoryID, taxPercent, 0, prepayCurrency, 1, BaseUnitCode, InvoiceTypeCodename, , ,,, "PREPAID", prepaidAmount)
        invoiceLine.PrepaidDocumentCode = "386" ' pass additional paramter to print on xml
        invoiceLine.PrepaidInvoiceId = PrepaidInvoiceId ' pass additional paramter to print on xml
        invoiceLine.PrepaidIssueDate = PrepaidInvoiceIssueDate ' pass additional paramter to print on xml
        invoiceLine.PrepaidIssueTime = PrepaidInvoiceIssueTime ' pass additional paramter to print on xml



        ' Add the new InvoiceLineItem object to the InvoiceLines list
        InvoiceLines.Add(invoiceLine)
        RecalculateDocument()
    End Sub




    'add document level allowances into the invoice
    Public Sub AddDocumentLevelAllowance(Reason As String, Amount As Decimal, TaxPercent As Decimal, TaxCategoryCode As String)
        DocumentLevelAllowances.Add(New DocumentLevelAllowance(Reason, Amount, TaxPercent, TaxCategoryCode))
        RecalculateDocument()
    End Sub


    'add document level charges into the invoice
    Public Sub AddDocumentLevelCharge(Reason As String, Amount As Decimal, TaxPercent As Decimal, TaxCategoryCode As String)
        DocumentLevelCharges.Add(New DocumentLevelCharge(Reason, Amount, TaxPercent, TaxCategoryCode))
        RecalculateDocument()
    End Sub


    Private Sub RecalculateDocument()
        ' Declare temporary Decimal variables for calculation
        Dim tempLineExtensionAmount As Decimal = 0
        Dim tempAllowanceTotalAmount As Decimal = 0
        Dim tempChargeTotalAmount As Decimal = 0
        Dim tempTotalTaxableAmount As Decimal = 0
        Dim tempTotalTaxAmount As Decimal = 0
        Dim tempTaxExclusiveAmount As Decimal = 0
        Dim tempTaxInclusiveAmount As Decimal = 0
        Dim tempPayableAmount As Decimal = 0
        Dim tempVATRate As Decimal = 0 ' Assuming VATRate is a string that should be converted
        Dim tempPrepaidAmount As Decimal = 0 ' Assuming PrepaidAmount is a string that should be converted
        Dim LineNetPrice As Decimal = 0
        Dim TotalLineTaxAmount As Decimal = 0

        Dim TempcatvatAmount As Decimal = 0
        Dim TempcatTaxableAmount As Decimal = 0

        Me.DocumentVatCategories = New List(Of UniqeTaxCategory)()
        ' Convert the string class-level variables to Decimal for calculation
        'tempLineExtensionAmount = Convert.ToDecimal(Me.LineExtensionAmount)
        'tempAllowanceTotalAmount = Convert.ToDecimal(Me.AllowanceTotalAmount)
        'tempChargeTotalAmount = Convert.ToDecimal(Me.ChargeTotalAmount)
        tempVATRate = Convert.ToDecimal(Me.VATRate)
        tempPrepaidAmount = Convert.ToDecimal(Me.PrepaidAmount)

        ' Iterate through InvoiceLines, converting string values to Decimal and summing
        If InvoiceLines IsNot Nothing Then
            For Each line In InvoiceLines
                tempLineExtensionAmount += Convert.ToDecimal(line.LineExtensionAmount)
                LineNetPrice = LineNetPrice + (Convert.ToDecimal(line.ItemPrice) * Convert.ToDecimal(line.InvoicedQuantity)) + Convert.ToDecimal(line.AllowanceChargeAmount)
                TotalLineTaxAmount = TotalLineTaxAmount + line.TaxAmount
                'document taxexcluve amount should not contain zero or E lines

                If line.TaxCategoryID = "S" Then
                    Dim foundItem As UniqeTaxCategory = DocumentVatCategories.Find(Function(x) x.VatCategoryCode = "S")
                    'tempTotalTaxableAmount += Convert.ToDecimal(line.LineExtensionAmount)
                    If foundItem IsNot Nothing Then
                        'update vat amount
                        TempcatvatAmount = Convert.ToDecimal(foundItem.CategoryVatAmount)
                        TempcatvatAmount += line.TaxAmount
                        foundItem.CategoryVatAmount = TempcatvatAmount
                        'update taxable amount
                        TempcatTaxableAmount = Convert.ToDecimal(foundItem.CatTaxableAmount)
                        TempcatTaxableAmount += Convert.ToDecimal(line.LineExtensionAmount)
                        foundItem.CatTaxableAmount = TempcatTaxableAmount
                    Else
                        Dim vc As New UniqeTaxCategory
                        vc.VatCategoryCode = "S"
                        vc.CategoryVatAmount = line.TaxAmount
                        vc.VatCategoryRate = line.TaxPercent
                        vc.CatTaxableAmount = Convert.ToDecimal(line.LineExtensionAmount)
                        ' create item
                        DocumentVatCategories.Add(vc)
                    End If
                End If

                If line.TaxCategoryID = "E" Then
                    Dim foundItem As UniqeTaxCategory = DocumentVatCategories.Find(Function(x) x.VatCategoryCode = "E")

                    If foundItem IsNot Nothing Then
                        'update vat amount
                        foundItem.CategoryVatAmount = "0.00"

                        'update taxable amount
                        TempcatTaxableAmount = Convert.ToDecimal(foundItem.CatTaxableAmount)
                        TempcatTaxableAmount += Convert.ToDecimal(line.LineExtensionAmount)
                        foundItem.CatTaxableAmount = TempcatTaxableAmount
                    Else
                        Dim vc As New UniqeTaxCategory
                        vc.VatCategoryCode = "E"
                        vc.CategoryVatAmount = "0.00"
                        vc.CatTaxableAmount = Convert.ToDecimal(line.LineExtensionAmount)
                        vc.VatExCode = line.VatExCode
                        vc.VatExDesc = line.VatExDescription
                        ' create item
                        DocumentVatCategories.Add(vc)
                    End If
                End If


                If line.TaxCategoryID = "Z" Then
                    Dim foundItem As UniqeTaxCategory = DocumentVatCategories.Find(Function(x) x.VatCategoryCode = "Z")

                    If foundItem IsNot Nothing Then
                        'update item'
                        foundItem.CategoryVatAmount = "0.00"


                        'update taxable amount
                        TempcatTaxableAmount = Convert.ToDecimal(foundItem.CatTaxableAmount)
                        TempcatTaxableAmount += Convert.ToDecimal(line.LineExtensionAmount)
                        foundItem.CatTaxableAmount = TempcatTaxableAmount
                    Else
                        Dim vc As New UniqeTaxCategory
                        vc.VatCategoryCode = "Z"
                        vc.CategoryVatAmount = "0.00"
                        vc.CatTaxableAmount = Convert.ToDecimal(line.LineExtensionAmount)
                        vc.VatExCode = line.VatExCode
                        vc.VatExDesc = line.VatExDescription

                        ' create item
                        DocumentVatCategories.Add(vc)
                    End If
                End If
            Next
        End If





        ' Iterate through DocumentLevelAllowances, converting string values to Decimal and summing
        If DocumentLevelAllowances IsNot Nothing Then
            For Each docAllowance In DocumentLevelAllowances
                tempAllowanceTotalAmount += Convert.ToDecimal(docAllowance.Amount)
            Next
        End If

        ' Iterate through DocumentLevelCharges, converting string values to Decimal and summing
        If DocumentLevelCharges IsNot Nothing Then
            For Each docCharge In DocumentLevelCharges
                tempChargeTotalAmount += Convert.ToDecimal(docCharge.Amount)
            Next
        End If

        ' adjust allowance charge in S vat category
        For Each cat In DocumentVatCategories
            'If cat.VatCategoryCode = "S" Then
            'cat.CategoryVatAmount = cat.CategoryVatAmount + (tempChargeTotalAmount - tempAllowanceTotalAmount) * tempVATRate / 100
            'ZTC 6/21
            Dim tempChargeTotalAmountForCat As Decimal = 0
            If DocumentLevelCharges IsNot Nothing Then
                For Each docCharge In DocumentLevelCharges
                    If docCharge.TaxCategoryCode = cat.VatCategoryCode Then
                        tempChargeTotalAmountForCat += Convert.ToDecimal(docCharge.Amount)
                    End If
                Next
            End If

            Dim tempAllowanceTotalAmountForCat As Decimal = 0
            If DocumentLevelAllowances IsNot Nothing Then
                For Each docAllowance In DocumentLevelAllowances
                    If docAllowance.TaxCategoryCode = cat.VatCategoryCode Then
                        tempAllowanceTotalAmountForCat += Convert.ToDecimal(docAllowance.Amount)
                    End If
                Next
            End If


            Dim categoryVatAmount As Decimal = (tempChargeTotalAmountForCat - tempAllowanceTotalAmountForCat) * cat.VatCategoryRate / 100
            cat.CategoryVatAmount = Math.Round(cat.CategoryVatAmount + categoryVatAmount, 2, MidpointRounding.AwayFromZero)
            cat.CatTaxableAmount = cat.CatTaxableAmount - tempAllowanceTotalAmountForCat + tempChargeTotalAmountForCat

            'get the sum of total tax amount for S
            If cat.VatCategoryCode = "S" Then
                tempTotalTaxAmount += cat.CategoryVatAmount
            End If

            tempTotalTaxableAmount += cat.CatTaxableAmount
        Next



        LineNetPrice = LineNetPrice - tempAllowanceTotalAmount + tempChargeTotalAmount 'adjust the line net prices  with document level charges and allowances

        ' Perform calculations
        'ZTC 6/21
        'tempTotalTaxableAmount = tempTotalTaxableAmount - tempAllowanceTotalAmount + tempChargeTotalAmount
        'tempTotalTaxAmount = TotalLineTaxAmount + (tempChargeTotalAmount - tempAllowanceTotalAmount) * tempVATRate / 100
        tempTaxExclusiveAmount = tempLineExtensionAmount - tempAllowanceTotalAmount + tempChargeTotalAmount
        tempTaxInclusiveAmount = tempTaxExclusiveAmount + tempTotalTaxAmount

        Dim tempPayableRoundingAmount As Decimal
        If Me.InvoiceTypeCodename = "0200000" Then ' payable rounding amount calculate only for simplified invoices
            tempPayableRoundingAmount = Math.Round(LineNetPrice - tempTaxInclusiveAmount, 2, MidpointRounding.AwayFromZero)
        Else
            tempPayableRoundingAmount = 0
        End If


        'tempPayableAmount = LineNetPrice - tempPrepaidAmount
        tempPayableAmount = tempTaxInclusiveAmount - tempPrepaidAmount + tempPayableRoundingAmount


        ' Store the calculated results back into the original string class-level variables,
        ' rounding them to two decimal places
        Me.LineExtensionAmount = tempLineExtensionAmount.ToString("F2")
        Me.AllowanceTotalAmount = tempAllowanceTotalAmount.ToString("F2")
        Me.ChargeTotalAmount = tempChargeTotalAmount.ToString("F2")
        Me.TotalTaxableAmount = tempTotalTaxableAmount.ToString("F2")
        Me.TotalTaxAmount = tempTotalTaxAmount.ToString("F2")
        Me.TaxExclusiveAmount = tempTaxExclusiveAmount.ToString("F2")
        Me.TaxInclusiveAmount = tempTaxInclusiveAmount.ToString("F2")
        Me.PayableAmount = tempPayableAmount.ToString("F2")
        Me.PayableRoundAmount = tempPayableRoundingAmount.ToString("F2")

    End Sub




    <Guid("E71DE678-758F-40E2-83D3-80311FB907CE")>
    Public Class Customer
        Public Property PartyName As String

        'all bellow for standard invoice
        Public Property VATNumber As String
        Public Property BuyerIdType As String
        Public Property BuyerIdNumber As String
        Public Property BuildingNo As String
        Public Property Street As String
        Public Property CitySubdivision As String
        Public Property PostalCode As String
        Public Property City As String

        Public Sub New()
        End Sub

        ' Parameterized Constructor
        Public Sub New(partyName As String,
            vatnumber As String,
            idType As String,
            idNumber As String,
            BuildingNo As String,
            street As String,
            citySubdivision As String,
            city As String,
            postalCode As String)

            Me.PartyName = partyName
            Me.VATNumber = vatnumber
            Me.BuyerIdType = idType
            Me.BuyerIdNumber = idNumber
            Me.BuildingNo = BuildingNo
            Me.Street = street
            Me.CitySubdivision = citySubdivision
            Me.PostalCode = postalCode
            Me.City = city

        End Sub

        Public Sub PopulateCustomer(partyName As String,
            vatnumber As String,
            idType As String,
            idNumber As String,
            BuildingNo As String,
            street As String,
            citySubdivision As String,
            city As String,
            postalCode As String)


            Me.PartyName = partyName
            Me.VATNumber = vatnumber
            Me.BuyerIdType = idType
            Me.BuyerIdNumber = idNumber
            Me.BuildingNo = BuildingNo
            Me.Street = street
            Me.CitySubdivision = citySubdivision
            Me.PostalCode = postalCode
            Me.City = city

        End Sub



    End Class




    <Guid("81C12AD8-4A44-49AA-848D-CCF7F1D49E2D")>
    Public Class Supplier
        Public Property IdType As String
        Public Property IdNo As String
        Public Property CompanName As String

        Public Property VATNumber As String
        Public Property StreetName As String

        Public Property BuildingNo As String

        Public Property PlotNumber As String

        Public Property CitySubdivision As String

        Public Property City As String

        Public Property PostalCode As String

        Public Sub New()
        End Sub

        ' Parameterized Constructor
        Public Sub New(
            idtype As String,
            idno As String,
            companName As String,
            vatnumber As String,
            streetName As String,
            buildingNo As String,
            plotNumber As String,
            citySubdivision As String,
            city As String,
            postalCode As String
        )
            Me.IdType = idtype
            Me.IdNo = idno
            Me.CompanName = companName
            Me.VATNumber = vatnumber
            Me.StreetName = streetName
            Me.BuildingNo = buildingNo
            Me.PlotNumber = plotNumber
            Me.CitySubdivision = citySubdivision
            Me.City = city
            Me.PostalCode = postalCode
        End Sub


        Public Sub PopulateSupplier(idtype As String,
            idno As String,
            companName As String,
            vatnumber As String,
            streetName As String,
            buildingNo As String,
            plotNumber As String,
            citySubdivision As String,
            city As String,
            postalCode As String)

            Me.IdType = idtype
            Me.IdNo = idno
            Me.CompanName = companName
            Me.VATNumber = vatnumber
            Me.StreetName = streetName
            Me.BuildingNo = buildingNo
            Me.PlotNumber = plotNumber
            Me.CitySubdivision = citySubdivision
            Me.City = city
            Me.PostalCode = postalCode

        End Sub


    End Class





    <Guid("CA0DFE23-4183-4C11-803A-DBC7933F1F4C")>
    Public Class DocumentLevelAllowance
        ' Define properties for DocumentLevelAllowance
        Public Property AllowanceChargeReason As String


        Public Property Amount As Decimal
        Public Property TaxPercent As Decimal

        Public Property TaxCategoryCode As String

        Public Sub New()

        End Sub

        ' parameterized Constructor to initialize the properties
        Public Sub New(allowanceReason As String, amount As Decimal, taxPercent As Decimal, TaxCategoryCode As String)
            Me.AllowanceChargeReason = allowanceReason

            Me.Amount = amount
            Me.TaxPercent = taxPercent
            Me.TaxCategoryCode = TaxCategoryCode
        End Sub




    End Class



    <Guid("454EC193-CAFC-4AF5-87E8-101F3B27D118")>
    Public Class DocumentLevelCharge
        ' Define properties for DocumentLevelAllowance
        Public Property AllowanceChargeReason As String


        Public Property Amount As Decimal
        Public Property TaxPercent As Decimal

        Public Property TaxCategoryCode As String

        ' parameterized  Constructor to initialize the properties
        Public Sub New(ChargeReason As String, amount As Decimal, taxPercent As Decimal, TaxCategoryCode As String)
            Me.AllowanceChargeReason = ChargeReason

            Me.Amount = amount
            Me.TaxPercent = taxPercent
            Me.TaxCategoryCode = TaxCategoryCode
        End Sub

    End Class


    <Guid("AD55F982-1B29-4980-B1BB-BEC8F684E233")>
    Public Class LineLevelAllowance
        Public Property Reason As String
        Public Property Amount As String

        Public Sub New(Reason As String, Amount As String)
            Me.Reason = Reason
            Me.Amount = Amount
        End Sub

    End Class


    <Guid("32210F53-AB6E-480C-88E0-9540ECF16127")>
    Public Class LineLevelCharge
        Public Property Reason As String
        Public Property Amount As String

        Public Sub New(Reason As String, Amount As String)
            Me.Reason = Reason
            Me.Amount = Amount
        End Sub
    End Class




End Class



<Guid("ACCA5C93-1FD2-4CBE-9FFD-FB7C1E7CF977")>
Public Class InvoiceLineItem
    Public Property LineID As String
    Public Property InvoicedQuantity As String
    Public Property UnitCode As String
    Public Property LineExtensionAmount As String
    Public Property TaxAmount As String
    Public Property RoundingAmount As String
    Public Property ItemName As String
    Public Property TaxCategoryID As String
    Public Property TaxPercent As String
    Public Property ItemPrice As String
    Public Property LineCurrency As String

    Public Property PriceAmount As String
    Public Property AllowanceChargeAmount As String

    Public Property BaseQuantity As String
    Public Property BaseUnitCode As String

    Public Property LineLevelCharges As List(Of LineLevelCharge)
    Public Property LineLevelAllowances As List(Of LineLevelAllowance)

    'type of invoice line, normal line or prepaid line
    Public Property InvoiceLineType As String = "REGULAR"
    'for prepaid invoice lines bello 4 attributes required
    Public Property PrepaidAmount As String
    Public Property PrepaidInvoiceId As String
    Public Property PrepaidIssueDate As String
    Public Property PrepaidIssueTime As String
    Public Property PrepaidDocumentCode As String
    Public Property PrepaidTaxSubTotalTaxableAmount As String
    Public Property PrepaidTaxSubTotalTaxAmount As String
    Public Property LineNetAmount As String 'purpose of printing on invoice

    'exception codes
    Public Property VatExCode As String
    Public Property VatExDescription As String

    Public Property InvoiceTypeCodename As String

    Public Property LibraryVersion As String = "1.1.0"

    Public Sub New()
    End Sub

    Public Sub New(InvoiceLineNo As String, quantity As String, UnitCode As String,
                  itemName As String, taxCategoryID As String, taxPercent As String,
                  itemPrice As String, pricecurrency As String, BaseQuantity As String, BaseUnitCode As String, InvoiceTypeCodeName As String, Optional Vatexcode As String = "", Optional Vatexdesc As String = "", Optional lineLevelAllowances As List(Of LineLevelAllowance) = Nothing,
                   Optional lineLevelCharges As List(Of LineLevelCharge) = Nothing, Optional invoiceLineType As String = "REGULAR", Optional prepaidAmount As String = "0")
        Me.LineID = InvoiceLineNo
        Me.InvoicedQuantity = quantity
        Me.UnitCode = UnitCode
        Me.ItemName = itemName
        Me.TaxCategoryID = taxCategoryID
        Me.TaxPercent = taxPercent
        Me.ItemPrice = itemPrice
        Me.BaseQuantity = BaseQuantity
        Me.BaseUnitCode = BaseUnitCode
        Me.InvoiceLineType = invoiceLineType
        Me.PrepaidAmount = prepaidAmount
        Me.LineCurrency = pricecurrency
        Me.InvoiceTypeCodename = InvoiceTypeCodeName
        Me.VatExCode = Vatexcode
        Me.VatExDescription = Vatexdesc


        ReCalculateLine()

        ' Initialize lists
        Me.LineLevelAllowances = If(lineLevelAllowances, New List(Of LineLevelAllowance)())
        Me.LineLevelCharges = If(lineLevelCharges, New List(Of LineLevelCharge)())
    End Sub

    Public Sub PopulateInvoiceLine(InvoiceLineNo As String, quantity As String, UnitCode As String,
                  itemName As String, taxCategoryID As String, taxPercent As String,
                  itemPrice As String, pricecurrency As String, BaseQuantity As String, BaseUnitCode As String, Optional Vatexcode As String = "", Optional Vatexdesc As String = "", Optional lineLevelAllowances As List(Of LineLevelAllowance) = Nothing,
                   Optional lineLevelCharges As List(Of LineLevelCharge) = Nothing, Optional invoiceLineType As String = "REGULAR", Optional prepaidAmount As String = "0")

        Me.LineID = InvoiceLineNo
        Me.InvoicedQuantity = quantity
        Me.UnitCode = UnitCode
        Me.ItemName = itemName
        Me.TaxCategoryID = taxCategoryID
        Me.TaxPercent = taxPercent
        Me.ItemPrice = itemPrice
        Me.BaseQuantity = BaseQuantity
        Me.BaseUnitCode = BaseUnitCode
        Me.InvoiceLineType = invoiceLineType
        Me.PrepaidAmount = prepaidAmount
        Me.LineCurrency = pricecurrency

        Me.VatExCode = Vatexcode
        Me.VatExDescription = Vatexdesc

        ReCalculateLine()

        ' Initialize lists
        Me.LineLevelAllowances = If(lineLevelAllowances, New List(Of LineLevelAllowance)())
        Me.LineLevelCharges = If(lineLevelCharges, New List(Of LineLevelCharge)())
    End Sub


    Public Sub ReCalculateLine()
        ' Convert string values to Decimal for calculations
        Dim itemPriceDecimal As Decimal = Convert.ToDecimal(Me.ItemPrice)
        Dim invoicedQuantityDecimal As Decimal = Convert.ToDecimal(Me.InvoicedQuantity)
        Dim taxPercentDecimal As Decimal = Convert.ToDecimal(Me.TaxPercent)
        Dim allowanceChargeAmountDecimal As Decimal = 0



        'If InvoiceLineType = "Regular" Then
        ' Subtract allowances
        If Me.LineLevelAllowances IsNot Nothing Then
            For Each allowance In Me.LineLevelAllowances
                allowanceChargeAmountDecimal -= Convert.ToDecimal(allowance.Amount)
            Next
        End If

        ' Add charges
        If Me.LineLevelCharges IsNot Nothing Then
            For Each charge In Me.LineLevelCharges
                allowanceChargeAmountDecimal += Convert.ToDecimal(charge.Amount)
            Next
        End If

        Dim ItempriceAmount As Decimal
        If InvoiceLineType = "REGULAR" Then

            ' CHANGED FOR TRIAL
            If Me.InvoiceTypeCodename = "0200000" Then 'If simplified price include a if standard vat is seperate
                ItempriceAmount = ((ItemPrice * invoicedQuantityDecimal) + allowanceChargeAmountDecimal) * 100 / (taxPercentDecimal + 100) / invoicedQuantityDecimal
            Else
                ItempriceAmount = ItemPrice
                'ItempriceAmount = ItemPrice + allowanceChargeAmountDecimal
            End If
            'ItempriceAmount = ((ItemPrice * invoicedQuantityDecimal) + allowanceChargeAmountDecimal) * 100 / (taxPercentDecimal + 100) / invoicedQuantityDecimal

        Else
            ItempriceAmount = 0
        End If

        'ItempriceAmount = Math.Round(ItempriceAmount, 2)
        Me.PriceAmount = ItempriceAmount.ToString("F2") ' save priceamount

        'Dim lineExtensionAmount As Decimal = ItempriceAmount * invoicedQuantityDecimal

        Dim lineExtensionAmount As Decimal
        If Me.InvoiceTypeCodename = "0200000" Then
            'lineExtensionAmount = (ItempriceAmount * invoicedQuantityDecimal)
            lineExtensionAmount = (ItempriceAmount * invoicedQuantityDecimal) / BaseQuantity
        Else
            'lineExtensionAmount = (ItempriceAmount * invoicedQuantityDecimal) + allowanceChargeAmountDecimal
            lineExtensionAmount = ((ItempriceAmount * invoicedQuantityDecimal) / BaseQuantity) + allowanceChargeAmountDecimal

        End If

        lineExtensionAmount = Math.Round(lineExtensionAmount, 2, MidpointRounding.AwayFromZero)
        Me.LineExtensionAmount = lineExtensionAmount.ToString("F2") ' save lineextensionamount

        ' Calculate TaxAmount and round to 2 decimal places
        Dim taxAmount As Decimal = lineExtensionAmount * taxPercentDecimal / 100
        'Dim taxAmount As Decimal = ItempriceAmount * invoicedQuantityDecimal * taxPercentDecimal / 100
        taxAmount = Math.Round(TaxAmount, 2, MidpointRounding.AwayFromZero)
        Me.TaxAmount = taxAmount.ToString("F2") 'save taxamount


        ' Calculate RoundingAmount and round to 2 decimal places
        Dim roundingAmount As Decimal = lineExtensionAmount + taxAmount
        roundingAmount = Math.Round(roundingAmount, 2, MidpointRounding.AwayFromZero)
        Me.RoundingAmount = roundingAmount.ToString("F2") 'save rounding amount
        Me.AllowanceChargeAmount = allowanceChargeAmountDecimal.ToString("F2") ' save this just to line calculation purposes
        Me.LineNetAmount = (lineExtensionAmount + taxAmount).ToString("F2") 'to print on invoice

        If InvoiceLineType = "PREPAID" Then
            Dim tempPrepaidAmount As Decimal = Convert.ToDecimal(Me.PrepaidAmount)
            PrepaidTaxSubTotalTaxableAmount = Math.Round((tempPrepaidAmount / (taxPercentDecimal + 100)) * 100, 2, MidpointRounding.AwayFromZero)

            PrepaidTaxSubTotalTaxAmount = Math.Round(PrepaidTaxSubTotalTaxableAmount * taxPercentDecimal / 100, 2, MidpointRounding.AwayFromZero)
            PrepaidTaxSubTotalTaxableAmount = tempPrepaidAmount - PrepaidTaxSubTotalTaxAmount
            Me.LineNetAmount = (-tempPrepaidAmount).ToString("F2") 'to print on invoice
        End If

    End Sub

    Public Sub AddLineLevelAllowance(Reason As String, Amount As String)
        Me.LineLevelAllowances.Add(New LineLevelAllowance(Reason, Amount))
        ReCalculateLine()
    End Sub

    Public Sub AddLineLevelCharge(Reason As String, Amount As String)
        Me.LineLevelCharges.Add(New LineLevelCharge(Reason, Amount))
        ReCalculateLine()
    End Sub


End Class

Public Class UniqeTaxCategory

    Public Property VatCategoryCode As String ' S or Z
    Public Property VatCategoryRate As String ' 0 or 15
    Public Property CatTaxableAmount As String
    Public Property CategoryVatAmount As String

    Public Property VatExCode As String
    Public Property VatExDesc As String


End Class

