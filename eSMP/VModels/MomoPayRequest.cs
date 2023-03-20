namespace eSMP.VModels
{
    public class MomoPayRequest
    {
        public string orderInfo { get; set; }
        public string partnerCode { get; set; }
        public string redirectUrl { get; set; }
        public string ipnUrl { get; set; }
        public long amount { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public string extraData { get; set; }
        public string partnerName { get; set; }
        public string requestType { get; set; }
        public string storeId { get; set; }
        public string paymentCode { get; set; }
        public string orderGroupId { get; set; }
        public bool autoCapture { get; set; }
        public string lang { get; set; }
        public string signature { get; set; }

    }
    public class MomoPayReponse
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public long amount { get; set; }
        public long responseTime { get; set; }
        public string message { get; set; }
        public int resultCode { get; set; }
        public string payUrl { get; set; }
        public string? deeplink { get; set; }
        public string? qrCodeUrl { get; set; }
        public string? deeplinkMiniApp { get; set; }
    }
    public class MomoPayINP
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public string orderInfo { get; set; }
        public string orderType { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string payType { get; set; }
        public long responseTime { get; set; }
        public string extraData { get; set; }
        public string signature { get; set; }
    }
    public class ConfirmRequest
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public long amount { get; set; }
        public string lang { get; set; }
        public string description { get; set; }
        public string requestType { get; set; }
        public string signature { get; set; }
    }
    public class ConfirmReponse
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string requestType { get; set; }
        public long responseTime { get; set; }
    }
    public class RefundReponse
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public long responseTime { get; set; }
    }
    public class RefundRequest
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public long transId { get; set; }
        public string lang { get; set; }
        public string description { get; set; }
        public string signature { get; set; }
    }

    public class infoRequest
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string signature { get; set; }
        public string lang { get; set; }
    }

    public class InfoReponse
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public string extraData { get; set; }
        public string payType { get; set; }
        public long amount { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public refundTrans[] refundTrans { get; set; }
        public string message { get; set; }
        public long responseTime { get; set; }
    }

    public class refundTrans
    {
        public string orderId { get; set; }
        public long amount { get; set; }
        public int resultCode { get; set; }
        public long transId { get; set; }
        public long createdTime { get; set; }
    }
}
