import type TenantSubscriptionResponse from "./TenantSubscriptionResponse";
import dayjs from "dayjs";

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
  statusName?: string,
  subscriptionStatus?: TenantSubscriptionResponse,
  tenantSubscriptionResponse?: TenantSubscriptionResponse

  createdBy?: string,
  createdOn?: dayjs.Dayjs
}