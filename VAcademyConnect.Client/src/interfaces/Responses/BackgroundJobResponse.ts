export default interface BackgroundJobResponse {
  id?: string;
  runDate: string;
  tenantId?: string;
  tenantName?: string;
  result?: string;
  logDetail?: string;
}
