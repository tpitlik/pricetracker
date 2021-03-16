using System;

namespace NlnrPriceDyn.DataAccess.Common.Models.ProductManagement
{
    public class ProductPriceInfoDB
    {
        public long Id { get; set; }
        public ProductDB Product { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public float MeanPrice { get; set; }
        public  int OffersCount { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
