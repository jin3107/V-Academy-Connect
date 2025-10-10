export enum Status {
  Actice = 0,
  Suspended = 1,
  Inactive = 2
}

export default interface TenantRequest {
  name: string,
  contactEmail: string,
  contactPhone: string,
  status: Status;
}