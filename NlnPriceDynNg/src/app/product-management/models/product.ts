export class Product {
  constructor(
    public Id: string,
    public Title: string,
    public LinkUrl: string,
    public MinPrice: string,
    public MaxPrice: string,
    public MeanPrice: string,
    public MinPriceChange: string,
    public MaxPriceChange: string,
    public MeanPriceChange: string,
    public OffersCount: string,
    public OutOfStock: boolean,
    public ProductNotificationId: string,
    public ProductNotificationActive: boolean
  ) { }
}
