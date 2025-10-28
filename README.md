# ZATCA E-Invoicing Integration Solution

A comprehensive ASP.NET MVC solution for Saudi Arabia's ZATCA (Zakat, Tax and Customs Authority) e-invoicing compliance.

## ?? Overview

This project provides a complete integration with ZATCA's e-invoicing system, supporting both simplified and standard tax invoices as per Saudi Arabia's Fatoora requirements.

## ? Features

- **ZATCA Compliance**: Full compliance with Phase 2 ZATCA e-invoicing requirements
- **Invoice Generation**: Create UBL 2.1 compliant XML invoices directly from database
- **Multiple Invoice Types**: 
  - Standard invoices (B2B)
  - Simplified invoices (B2C)
  - Credit notes
  - Debit notes
- **CSID Management**: 
  - Compliance CSID generation
  - Production CSID onboarding
  - CSID renewal
- **QR Code Generation**: Automatic ZATCA-compliant QR code generation
- **Invoice Validation**: Compliance check before submission
- **API Integration**: RESTful API endpoints for all ZATCA operations

## ??? Technology Stack

- **Framework**: ASP.NET MVC (.NET Framework 4.6.1)
- **Language**: C# 13.0
- **Database**: Entity Framework with SQL Server
- **XML Processing**: System.Xml, UBL 2.1
- **Security**: BouncyCastle for cryptographic operations
- **QR Codes**: ZXing library
- **JSON**: Newtonsoft.Json

## ?? Prerequisites

- Visual Studio 2022 or later
- .NET Framework 4.6.1 or higher
- SQL Server 2012 or later
- ZATCA SDK (included)

## ?? Getting Started

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/ZatcaSolution.git
cd ZatcaSolution
```

2. Open the solution in Visual Studio:
```
TpService.sln
```

3. Restore NuGet packages:
```
Tools > NuGet Package Manager > Restore NuGet Packages
```

4. Update database connection string in `Web.config`

5. Build and run the solution

### Configuration

Update the following in your configuration:
- Database connection string
- ZATCA API endpoints (developer/simulation/production)
- Company registration details
- Device information

## ?? API Endpoints

### Invoice Operations

- `GET /Zatca/getInvoices` - Retrieve all invoices
- `GET /Zatca/getInvoiceDetails?invId={id}` - Get invoice details
- `POST /Zatca/PostInvoice` - Submit invoice to ZATCA
- `POST /Zatca/TestInvoices` - Test invoice generation
- `POST /Zatca/ComplianceInvoices` - Compliance check

### CSID Operations

- `POST /Zatca/ComplianceCSID` - Get compliance CSID
- `POST /Zatca/ProductionCSIDOnBoarding` - Onboard production CSID
- `POST /Zatca/ProductionCSIDRenewal` - Renew production CSID

## ?? Key Components

### Controllers
- `ZatcaController.cs` - Main controller for ZATCA operations

### Models
- `Invoice.cs` - UBL invoice model
- Database entities for sales, products, taxes, payments

### Services
- `UBLXML.cs` - XML generation and signing
- `GenerateXml()` - Direct XML generation from database

## ?? Invoice Generation Methods

### Method 1: Using Invoice Object (Traditional)
```csharp
var invoice = ZatcaSalesInvoice(invoiceId, invoiceType);
var result = ubl.GenerateInvoiceXML(invoice, path, saveFile);
```

### Method 2: Direct XML Generation (New)
```csharp
XmlDocument xml = GenerateXml(invoiceId, invoiceType);
// Process XML directly
```

## ?? Security

- Private key management for invoice signing
- Certificate-based authentication with ZATCA
- Secure storage of CSID credentials
- HTTPS enforcement for API calls

## ?? Database Schema

Key tables:
- `ScSalesInvoices` - Sales invoices
- `ScSubSalesInvoices` - Invoice line items
- `CgMngDevicesZatcas` - ZATCA device credentials
- `MyPortalWebsites` - Company information
- `ScClients` - Customer information

## ?? Testing

Use the built-in test endpoints:
- `/Zatca/CheckPostMan` - API connectivity test
- `/Zatca/TestInvoices` - Invoice object generation test
- `/Zatca/TestInvoices2` - Full compliance test

## ?? Documentation

- [ZATCA E-Invoicing Portal](https://zatca.gov.sa/en/E-Invoicing/Pages/default.aspx)
- [UBL 2.1 Specification](http://docs.oasis-open.org/ubl/UBL-2.1.html)
- Project Wiki (coming soon)

## ?? Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## ?? License

This project is licensed under the MIT License - see the LICENSE file for details.

## ?? Acknowledgments

- ZATCA for their e-invoicing initiative
- Saudi Arabia's digital transformation vision
- Open source community for libraries used

## ?? Support

For issues and questions:
- Open an issue on GitHub
- Email: support@yourdomain.com

## ??? Roadmap

- [ ] Add unit tests
- [ ] Implement batch invoice processing
- [ ] Add invoice reporting dashboard
- [ ] Support for additional invoice types
- [ ] Multi-language support
- [ ] Docker containerization

---

**Version**: 1.0.0  
**Last Updated**: October 2024  
**Status**: Active Development
