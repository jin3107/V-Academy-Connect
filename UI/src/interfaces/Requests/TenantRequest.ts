import type { Status } from "../../commons";

export default interface TenantRequest {
  name: string,
  contactEmail: string,
  contactPhone: string,
  status: Status;
}