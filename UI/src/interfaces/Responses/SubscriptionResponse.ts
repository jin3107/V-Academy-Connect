import dayjs from "dayjs";

export enum BillingCycle {
  None = 0,
  Monthlyy = 1,
  Yearly = 2
}

export default interface SubscriptionResponse {
  id?: string,
  name: string,
  price: number,
  billingCycle: BillingCycle,
  trialDays: number,

  createdBy?: string,
  createdOn?: dayjs.Dayjs
}