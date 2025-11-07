using Newtonsoft.Json;

namespace OvulaeApp.Models.IOS
{
    public class ReceiptVerificationResult
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("receipt")]
        public ReceiptInfo Receipt { get; set; }
    }
}
