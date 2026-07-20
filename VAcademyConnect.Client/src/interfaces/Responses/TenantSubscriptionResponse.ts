import dayjs from "dayjs"

export default interface TenantSubscriptionResponse {
  hasActiveSubscription: boolean,
  isExpiringSoon: boolean,
  isExpired: boolean,
  expirationDate?: Date | string,
  subscriptionPlanName: string,
  status: string,

  createdBy?: string,
  createdOn?: dayjs.Dayjs
}