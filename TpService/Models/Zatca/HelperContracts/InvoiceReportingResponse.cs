namespace ZatcaIntegrationSDK.HelperContracts
{
    public class InvoiceReportingResponse
    {
        public ValidationResults validationResults { get; set; }
        public string ReportingStatus { get; set; }
        public string ClearanceStatus { get; set; }
        public object QrSellertStatus { get; set; }
        public object QrBuyertStatus { get; set; }
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
        
       
    }
}
