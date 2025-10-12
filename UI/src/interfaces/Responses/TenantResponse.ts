import type { Status } from "../../commons";
import type TenantSubscriptionResponse from "./TenantSubscriptionResponse";
import dayjs from "dayjs";


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