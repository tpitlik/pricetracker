export class CreateUpdateProductNotificationRequest {
  constructor(
    public Id: string,
    public IsActive: boolean,
    public TriggerOnce: boolean,
    public LowPriceLimit: number
  ) { }
}
