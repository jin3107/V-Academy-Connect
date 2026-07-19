export default interface PaymentResponse {
  id?: string;
  studentId?: string;
  studentName?: string;
  amount: number;
  note?: string;
  transferTime?: string;
  qrInfo?: string;
}
