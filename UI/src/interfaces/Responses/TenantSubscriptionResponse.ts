export default interface TenantSubscriptionResponse {
  hasActiveSubscription: boolean,
  isExpiringSoon: boolean,
  isExpired: boolean,
  expirationDate?: Date,
  subscriptionPlanName: string,
  status: string
}