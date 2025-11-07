namespace OvulaeApp.Models.IOS
{
    public class InAppPurchase
    {
        public string ProductId { get; set; }
        public string ProductType { get; set; } // "subscription" or "consumable"
        public string Title { get; set; }
        public string Description { get; set; }
        public string LocalizedPrice { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsPurchased { get; set; }
    }
}
