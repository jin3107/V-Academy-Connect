export enum BillingCycle {
  None = 0,
  Monthly = 1,
  Yearly = 2
}

class Subscription {
  public id: string | null;
  public name: string;
  public price: number;
  public billingCycle: BillingCycle;
  public trialDays: number;
  
  public constructor(id: string, name: string, price: number, billingCycle: BillingCycle, trialDays: number) {
    this.id = id;
    this.name = name;
    this.price = price;
    this.billingCycle = billingCycle;
    this.trialDays = trialDays;
  }
}

export default Subscription;