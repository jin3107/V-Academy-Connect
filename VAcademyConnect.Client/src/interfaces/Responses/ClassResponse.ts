export default interface ClassResponse {
  id?: string;
  name: string;
  tuition: number;
  teacherName?: string;
  tenantName?: string;

  createdOn: Date;
  createdBy: string;
}