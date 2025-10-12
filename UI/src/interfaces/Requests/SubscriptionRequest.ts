import type { BillingCycle } from "../../commons/enums";

export default interface SubscriptionRequest {
  name: string,
  price: number,
  billingCycle: BillingCycle,
  trialDays: number,
}