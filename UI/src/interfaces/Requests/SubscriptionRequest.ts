export enum BillingCycle {
  None = 0,
  Monthly = 1,
  Yearly = 2
}

export default interface SubscriptionRequest {
  name: string,
  price: number,
  billingCycle: BillingCycle,
  trialDays: number,
}