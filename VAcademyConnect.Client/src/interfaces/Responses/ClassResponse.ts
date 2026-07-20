export default interface ClassResponse {
  id?: string;
  name: string;
  tenantId?: string;
  tuition: number;
  teacherId?: string;
  teacherName?: string;
  tenantName?: string;
  teachingDayOfWeek?: number;
  teachingStartTime?: string;
  teachingEndTime?: string;

  createdOn: Date;
  createdBy: string;
}
