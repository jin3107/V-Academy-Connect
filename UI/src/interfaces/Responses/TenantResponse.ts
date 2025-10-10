import type TenantSubscriptionResponse from "./TenantSubscriptionResponse";

export enum Status {
  Active = 0,
  Suspended = 1,
  Inactive = 2
}

export default interface TenantResponse {
  id?: string,
  name: string,
  contactEmail: string,
  contactPhone: string,
  status: Status,
  //statusName: string
  tenantSubscriptionResponse: TenantSubscriptionResponse
}