using System;
using System.Collections.Generic;
using System.Text;

namespace NlnrPriceDyn.DataAccess.Common.Models.ProductManagement
{
    public class ProductDB
    {
        public string Id { get; set; }
        public string CatalogueId { get; set; }
        public string CatalogueName { get; set; }
        public DateTime Added { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public float MeanPrice { get; set; }
        public float MinPriceChange { get; set; }
        public float MeanPriceChange { get; set; }
        public float MaxPriceChange { get; set; }
        public int OffersCount { get; set; }
        public int RefsCount { get; set; }
        public bool OutOfStock { get; set; }
        public ICollection<ProductPriceInfoDB> PriceList { get; set; }
    }
}
