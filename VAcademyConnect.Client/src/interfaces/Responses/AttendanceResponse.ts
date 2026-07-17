export default interface AttendanceResponse {
  id?: string;
  classId?: string;
  className?: string;
  studentId?: string;
  studentName?: string;
  date?: string;
  isPresent: boolean;
  countAsPaid: boolean;
}
