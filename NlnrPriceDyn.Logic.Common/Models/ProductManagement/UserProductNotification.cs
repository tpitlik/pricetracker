namespace NlnrPriceDyn.Logic.Common.Models.ProductManagement
{
    public class UserProductNotification
    {
        public string Id { get; set; }
        public string ProductTitle { get; set; }
        public bool IsActive { get; set; }
        public bool TriggerOnce { get; set; }
        public float LowPriceLimit { get; set; }
        public float CurrentMinPrice { get; set; }
    }
}