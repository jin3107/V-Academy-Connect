import dayjs from "dayjs";
import type { BillingCycle } from "../../commons/enums";


export default interface SubscriptionResponse {
  id?: string,
  name: string,
  price: number,
  billingCycle: BillingCycle,
  trialDays: number,

  createdBy?: string,
  createdOn?: dayjs.Dayjs
}