class TenantSubscription {
  public id: string | null;
  public startDate: Date;
  public endDate: Date;
  public isTrial: boolean;

  public constructor(id: string, startDate: Date, endDate: Date, isTrial: boolean) {
    this.id = id;
    this.startDate = startDate;
    this.endDate = endDate;
    this.isTrial = isTrial;
  }
}

export default TenantSubscription;