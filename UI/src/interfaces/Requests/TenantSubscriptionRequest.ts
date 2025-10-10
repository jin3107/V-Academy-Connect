export default interface TenantSubscriptionRequest {
  id: string,
  subscriptionId: string,
  startDate: Date,
  endDate: Date,
  isTrial: boolean,
}