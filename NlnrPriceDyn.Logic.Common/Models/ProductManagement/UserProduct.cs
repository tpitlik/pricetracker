namespace NlnrPriceDyn.Logic.Common.Models.ProductManagement
{
    public class UserProduct
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string LinkUrl { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string MeanPrice { get; set; }
        public string MinPriceChange { get; set; }
        public string MeanPriceChange { get; set; }
        public string MaxPriceChange { get; set; }
        public int OffersCount { get; set; }
        public bool OutOfStock { get; set; }
        public string ProductNotificationId { get; set; }
        public bool ProductNotificationActive { get; set; }
        public bool ProductNotificationTriggerOnce { get; set; }
    }
}
