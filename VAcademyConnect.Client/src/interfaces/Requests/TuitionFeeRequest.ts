export default interface TuitionFeeRequest {
  id?: string;
  studentId: string;
  month: string;
  amount: number;
  dueDate?: string;
}
