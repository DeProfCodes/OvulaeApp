using Newtonsoft.Json;

namespace OvulaeApp.Models.IOS
{
    public class ReceiptInfo
    {
        [JsonProperty("in_app")]
        public List<InAppPurchase> InAppPurchases { get; set; }
    }
}
