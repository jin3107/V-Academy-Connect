import type { TenantType } from "../../../commons";

export default interface LogInResponse {
  email: string;
  token: string;
  phoneNumber: string;
  userId: string;
  tenantId: string;
  tenantType: TenantType;
  role: string;
  userName: string
};