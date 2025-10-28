using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpService.Models.Data
{
    public enum PaymentType
    {

        Cash = 1,
        Visa = 2,
        Master = 3,
        Mada = 4,
        Remain = 5,
        Qitaf = 6,
        STCPay = 7,
        Points = 8,
        Coupon = 9
    }
    public enum ShippingProcess
    {
        Preparing = 1,
        Shipping = 2,
        Delivery = 3,
        Packaging = 4,
        Receiving = 5,
        Damaged = 6,
        Waiting = 7
    }
    public enum OperationType
    {
        Transfer = 0,
        Sales = 1,
        Buys = 2,
        Jard = 3,
        ReturnSales = 4,
        ReturnBuys = 5,
        Talef = 6,
        RecVoucher = 7,
        InvOut = 8,
        InvIn = 9,
        POS = 40
    }
    //public enum RequestType
    //{
    //    Waiting = 1,
    //    Preparing = 2,
    //    Shipping = 3,
    //    Received = 4,
    //    Reject = 5,
    //    NotReceived = 6,
    //    CheckTranferBalance = 7
    //}
    public enum PaymentTransaction
    {
        Visa = 1,
        Sadad = 2,
        MasterCard = 3,
        Transfeer = 4,
        Cash = 5
    }
    public enum PaymentStatus
    {
        Complete = 1,
        NotCompete = 2
    }
    public enum TransferStatus
    {
        Complete = 1,
        NotCompete = 2
    }
    public enum ClientsType
    {
        StoreClient = 1,
        POSClient = 2,
        GomlaClient = 3
    }



}