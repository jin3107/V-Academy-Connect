import type { TenantType } from "../../../commons";

export default interface RegisterResponse {
  name: string,
  email: string,
  phoneNumber: string,
  tenantName: string,
  tenantType: TenantType,
  subscriptionPlanId: string,
  token: string;
}