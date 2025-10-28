namespace ZatcaIntegrationSDK.HelperContracts
{
    public class InvoiceClearanceResponse
    {
        public ValidationResults validationResults { get; set; }
        public string ClearanceStatus { get; set; }
        public string ClearedInvoice { get; set; }
        public string QRCode { get; set; }
        public string SingedXML { get; set; }
        
        public string ErrorMessage { get; set; }
        public string WarningMessage
        {
            get
            {
                return validationResults.WarningMessages.ToWarnings();
            }
        }

        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string ClearedXMLFileName { get; set; }
        public string ClearedXMLFileNameFullPath { get; set; }
        public string ClearedXMLFileNameShortPath { get; set; }
    }
}
