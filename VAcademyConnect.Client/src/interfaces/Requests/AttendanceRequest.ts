export default interface AttendanceRequest {
  id?: string;
  classId: string;
  studentId: string;
  date: string;
  isPresent: boolean;
  countAsPaid: boolean;
}
