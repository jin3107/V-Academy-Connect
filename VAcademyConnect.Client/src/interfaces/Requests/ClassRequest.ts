export default interface ClassRequest {
  id?: string;
  name: string;
  tenantId?: string;
  tuition: number;
  teacherId?: string;
  teachingDayOfWeek?: number;
  teachingStartTime?: string;
  teachingEndTime?: string;
}
