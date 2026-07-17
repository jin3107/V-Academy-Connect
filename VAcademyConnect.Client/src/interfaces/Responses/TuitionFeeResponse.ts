export default interface TuitionFeeResponse {
  id?: string;
  studentId?: string;
  studentName?: string;
  email?: string;
  month: string;
  amount: number;
  dueDate?: string;
}
