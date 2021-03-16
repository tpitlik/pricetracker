export class ProductNotification {
  constructor(
    public Id: string,
    public ProductTitle: string,
    public IsActive: boolean,
    public TriggerOnce: boolean,
    public LowPriceLimit: number,
    public CurrentMinPrice: number
  ) { }
}
